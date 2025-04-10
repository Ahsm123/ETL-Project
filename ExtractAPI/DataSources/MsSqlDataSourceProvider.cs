using ETL.Domain.Model;
using ETL.Domain.Sources;
using ETL.Domain.SQLQueryBuilder.Interfaces;
using ExtractAPI.DataSources.DatabaseQueryBuilder.Interfaces;
using ExtractAPI.Interfaces;
using System.Text.Json;

public class MsSqlDataSourceProvider : IDataSourceProvider
{
    private readonly ISqlExecutor _executor;
    private readonly ISqlQueryBuilder _queryBuilder;

    public MsSqlDataSourceProvider(ISqlExecutor executor, ISqlQueryBuilder queryBuilder)
    {
        _executor = executor;
        _queryBuilder = queryBuilder;
    }

    public bool CanHandle(Type sourceInfoType) =>
        typeof(MsSqlSourceInfo).IsAssignableFrom(sourceInfoType);

    public async Task<JsonElement> GetDataAsync(ExtractConfig config)
    {
        if (config.SourceInfo is not MsSqlSourceInfo dbInfo)
            throw new ArgumentException("Invalid sourceInfo: must be of type MsSqlSourceInfo");

        var (query, parameters) = _queryBuilder.BuildSelectQuery(dbInfo, config.Fields, config.Filters ?? new());

        var rows = await _executor.ExecuteQueryAsync(dbInfo.ConnectionString, query, parameters);
        var json = JsonSerializer.Serialize(rows);

        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.Clone();
    }
}
