using ETL.Domain.NewFolder;
using ETL.Domain.Rules;
using ETL.Domain.SQLQueryBuilder.Interfaces;
using ETL.Domain.Targets;
using ETL.Domain.Targets.DbTargets;
using Load.Interfaces;
using MySqlConnector;
using MySqlX.XDevAPI.Relational;
using System.Text.Json;

namespace Load.Writers;

public class MySqlTargetWriter : ITargetWriter
{
    private readonly IMySqlQueryBuilder _queryBuilder;
    private readonly IMySqlExecutor _executor;

    public MySqlTargetWriter(IMySqlQueryBuilder queryBuilder, IMySqlExecutor executor)
    {
        _queryBuilder = queryBuilder;
        _executor = executor;
    }

    public bool CanHandle(Type targetInfoType) =>
        typeof(MySqlTargetInfo).IsAssignableFrom(targetInfoType);

    public async Task WriteAsync(LoadContext context)
    {
        if (context.TargetInfo is not MySqlTargetInfo info)
            throw new ArgumentException("Invalid target info type");


        foreach (var table in context.Tables)
        {
            var mappedData = ApplyTargetMappings(context.Data, table.Fields);
            var (sql, parameters) = _queryBuilder.GenerateInsertQuery(table.TargetTable, mappedData);

            await _executor.ExecuteQueryAsync(BuildConnectionString(info), sql, parameters);
        }
    }

    private static Dictionary<string, object> ApplyTargetMappings(
        Dictionary<string, object> data,
        List<LoadFieldMapRule> mappings)
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

    private string BuildConnectionString(MySqlTargetInfo info)
    {
        var builder = new MySqlConnectionStringBuilder(info.ConnectionString)
        {
            AllowPublicKeyRetrieval = true // fix for caching_sha2_password
        };

        return builder.ConnectionString;
    }
}
