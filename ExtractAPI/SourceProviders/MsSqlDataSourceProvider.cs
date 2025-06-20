﻿using ETL.Domain.Model;
using ETL.Domain.Sources;
using ETL.Domain.SQLQueryBuilder.Interfaces;
using ExtractAPI.SourceProviders.Interfaces;
using System.Text.Json;

namespace ExtractAPI.DataSources;
public class MsSqlDataSourceProvider : IDataSourceProvider
{
    private readonly IMsSqlExecutor _executor;
    private readonly IMsSqlQueryBuilder _queryBuilder;

    public MsSqlDataSourceProvider(IMsSqlExecutor executor, IMsSqlQueryBuilder queryBuilder)
    {
        _executor = executor;
        _queryBuilder = queryBuilder;
    }

    public bool CanHandle(Type sourceInfoType) =>
        typeof(MsSqlSourceInfo).IsAssignableFrom(sourceInfoType);

    public async Task<JsonElement> GetDataAsync(ExtractConfig config)
    {
        var dbInfo = ValidateSourceInfo(config);

        if (dbInfo.JoinConfig != null)
        {
            var (query, parameters) = _queryBuilder.GenerateJoinedSelectQuery(dbInfo.JoinConfig);
            Console.WriteLine("Generated SQL:\n" + query);

            var rows = await _executor.ExecuteQueryAsync(dbInfo.ConnectionString, query, parameters);

            var json = JsonSerializer.Serialize(rows);
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.Clone();
        }
        else
        {
            var (query, parameters) = _queryBuilder.GenerateSelectQuery(
                dbInfo,
                config.Fields ?? new(),
                config.Filters ?? new()
            );
            var rows = await _executor.ExecuteQueryAsync(dbInfo.ConnectionString, query, parameters);

            var json = JsonSerializer.Serialize(rows);
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.Clone();
        }
    }


    private static MsSqlSourceInfo ValidateSourceInfo(ExtractConfig config)
    {
        if (config.SourceInfo is not MsSqlSourceInfo dbInfo)
            throw new ArgumentException("Invalid sourceInfo: must be of type MsSqlSourceInfo");
        return dbInfo;
    }
}
