using ETL.Domain.MetaDataModels;
using ETL.Domain.NewFolder;
using ETL.Domain.Rules;
using ETL.Domain.SQLQueryBuilder.Interfaces;
using ETL.Domain.Targets;
using ETL.Domain.Targets.DbTargets;
using Load.Services.Interfaces;
using Load.TargetWriters.Interfaces;

namespace Load.Writers;

public class MsSqlTargetWriter : ITargetWriter
{
    private readonly IMsSqlQueryBuilder _queryBuilder;
    private readonly IMsSqlExecutor _executor;
    private readonly IDataBaseMetadataService _metadataService;
    private readonly ITableDependencySorter _tableDependencySorter;

    public MsSqlTargetWriter(IMsSqlQueryBuilder queryBuilder,
        IMsSqlExecutor executor,
        IDataBaseMetadataService metadataService,
        ITableDependencySorter tableDependencySorter)
    {
        _queryBuilder = queryBuilder;
        _executor = executor;
        _metadataService = metadataService;
        _tableDependencySorter = tableDependencySorter;
    }

    public bool CanHandle(Type targetInfoType)
        => typeof(MsSqlTargetInfo).IsAssignableFrom(targetInfoType);

    public async Task WriteAsync(LoadContext context)
    {
        if (context.TargetInfo is not MsSqlTargetInfo info)
            throw new ArgumentException("Invalid target info type");

        if (info.UseBulkInsert)
            throw new NotImplementedException("Bulk insert is not implemented yet.");

        if (context.DatabaseMetadata == null && context.TargetInfo is DbTargetInfoBase dbTarget)
        {
            context.DatabaseMetadata = await _metadataService.FetchAsync(dbTarget, "mssql");
        }

        var sortedTables = _tableDependencySorter.SortByDependency(context.Tables, context.DatabaseMetadata);

        foreach (var table in sortedTables)
        {
            try
            {
                var mappedData = ApplyTargetMappings(context.Data, table.Fields);
                ValidateRequiredFields(table.TargetTable, mappedData, context.DatabaseMetadata);
                var (sql, parameters) = _queryBuilder.GenerateInsertQuery(table.TargetTable, mappedData);

                await _executor.ExecuteQueryAsync(info.ConnectionString, sql, parameters);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }

    private Dictionary<string, object> ApplyTargetMappings(Dictionary<string, object> data, List<FieldMapRule> mappings)
    {
        if (mappings == null || mappings.Count == 0) return data;

        var mapped = new Dictionary<string, object>();
        foreach (var map in mappings)
        {
            if (data.TryGetValue(map.SourceField, out var value))
            {
                mapped[map.TargetField] = value;
            }
        }
        return mapped;
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
                // Check if value is missing or null or empty string
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
}
