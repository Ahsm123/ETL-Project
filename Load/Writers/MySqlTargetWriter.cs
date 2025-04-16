using ETL.Domain.Rules;
using ETL.Domain.SQLQueryBuilder.Interfaces;
using ETL.Domain.Targets;
using ETL.Domain.Targets.DbTargets;
using Load.Interfaces;
using MySqlConnector;
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

    public async Task WriteAsync(TargetInfoBase targetInfo, Dictionary<string, object> data, string? pipelineId = null)
    {
        if (targetInfo is not MySqlTargetInfo info)
            throw new ArgumentException("Invalid target info type");

        var mappedData = ApplyTargetMappings(data, info.TargetMappings);
        var (sql, parameters) = _queryBuilder.GenerateInsertQuery(info, mappedData);

        try
        {
            await _executor.ExecuteQueryAsync(BuildConnectionString(info), sql, parameters);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to write to MySQL target: {ex.Message}", ex);
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
                mapped[map.TargetColumn] = NormalizeValue(value);
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
            SslMode = info.UseSsl ? MySqlSslMode.Required : MySqlSslMode.None,
            AllowPublicKeyRetrieval = true // fix for caching_sha2_password
        };

        return builder.ConnectionString;
    }
}
