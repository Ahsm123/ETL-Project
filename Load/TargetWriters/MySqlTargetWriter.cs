using Confluent.Kafka;
using ETL.Domain.MetaDataModels;
using ETL.Domain.NewFolder;
using ETL.Domain.Rules;
using ETL.Domain.SQLQueryBuilder.Interfaces;
using ETL.Domain.Targets;
using ETL.Domain.Targets.DbTargets;
using Load.Services.Interfaces;
using Load.TargetWriters.Interfaces;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using System.Text.Json;

namespace Load.Writers;

public class MySqlTargetWriter : ITargetWriter
{
    private readonly IMySqlQueryBuilder _queryBuilder;
    private readonly IMySqlExecutor _executor;
    private readonly IDataBaseMetadataService _metadataService;
    private readonly ITableDependencySorter _tableDependencySorter;

    public MySqlTargetWriter(
        IMySqlQueryBuilder queryBuilder,
        IMySqlExecutor executor,
        IDataBaseMetadataService metadataService,
        ITableDependencySorter tableDependencySorter)
    {
        _queryBuilder = queryBuilder;
        _executor = executor;
        _metadataService = metadataService;
        _tableDependencySorter = tableDependencySorter;
    }

    public bool CanHandle(Type targetInfoType) =>
        typeof(MySqlTargetInfo).IsAssignableFrom(targetInfoType);

    public async Task WriteAsync(LoadContext context)
    {
        if (context.TargetInfo is not MySqlTargetInfo info)
            throw new ArgumentException("Invalid target info type");

        if (context.DatabaseMetadata == null && context.TargetInfo is DbTargetInfoBase dbTarget)
        {
            context.DatabaseMetadata = await _metadataService.FetchAsync(dbTarget, "mysql");
        }

        var sortedTables = _tableDependencySorter.SortByDependency(context.Tables, context.DatabaseMetadata);

        var identityMap = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        foreach (var table in sortedTables)
        {
            try
            {
                var mappedData = ApplyTargetMappings(context.Data, table.Fields);

                var tableMeta = context.DatabaseMetadata.Tables
                    .FirstOrDefault(t => t.TableName.Equals(table.TargetTable, StringComparison.OrdinalIgnoreCase));

                if (tableMeta?.ForeignKeys != null)
                {
                    foreach (var fk in tableMeta.ForeignKeys)
                    {
                        if (identityMap.TryGetValue(fk.ReferencedTable, out var parentId))
                        {
                            mappedData[fk.Column] = parentId!;
                        }
                    }
                }

                ValidateRequiredFields(table.TargetTable, mappedData, context.DatabaseMetadata);

                var (sql, parameters) = _queryBuilder.GenerateInsertQuery(table.TargetTable, mappedData);

                bool hasAutoIncrement = tableMeta?.Columns
                    .Any(c => c.IsPrimaryKey && c.IsAutoIncrement) == true;

                if (hasAutoIncrement)
                {
                    var insertedId = await _executor.ExecuteInsertWithIdentityAsync(
                        BuildConnectionString(info), sql, parameters);
                    identityMap[table.TargetTable] = insertedId!;
                }
                else
                {
                    await _executor.ExecuteQueryAsync(BuildConnectionString(info), sql, parameters);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }

    private static Dictionary<string, object> ApplyTargetMappings(
        Dictionary<string, object> data,
        List<FieldMapRule> mappings)
    {
        if (mappings == null || mappings.Count == 0)
            return data;

        var mapped = new Dictionary<string, object>();

        foreach (var map in mappings)
        {
            if (data.TryGetValue(map.SourceField, out var value))
            {
                mapped[map.TargetField] = NormalizeValue(value);
            }
        }

        return mapped;
    }

    private static object NormalizeValue(object value)
    {
        if (value is JsonElement json)
        {
            return json.ValueKind switch
            {
                JsonValueKind.String => json.GetString() ?? "",
                JsonValueKind.Number => json.TryGetInt64(out var i) ? i : json.GetDouble(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                _ => json.ToString() ?? ""
            };
        }

        return value;
    }

    private void ValidateRequiredFields(string tableName, Dictionary<string, object> mappedData, DatabaseMetaData metadata)
    {
        var tableMeta = metadata.Tables
            .FirstOrDefault(t => t.TableName.Equals(tableName, StringComparison.OrdinalIgnoreCase));

        if (tableMeta == null)
            throw new InvalidOperationException($"No metadata found for table '{tableName}'.");

        var missingFields = new List<string>();

        foreach (var column in tableMeta.Columns)
        {
            if (!column.IsNullable && !column.IsAutoIncrement)
            {
                if (!mappedData.TryGetValue(column.ColumnName, out var value) ||
                    value == null ||
                    (value is string str && string.IsNullOrWhiteSpace(str)))
                {
                    missingFields.Add(column.ColumnName);
                }
            }
        }

        if (missingFields.Count > 0)
        {
            var missing = string.Join(", ", missingFields);
            throw new InvalidOperationException($"Missing required field(s) for table '{tableName}': {missing}");
        }
    }

    private string BuildConnectionString(MySqlTargetInfo info)
    {
        var builder = new MySqlConnectionStringBuilder(info.ConnectionString)
        {
            AllowPublicKeyRetrieval = true
        };

        return builder.ConnectionString;
    }
}
