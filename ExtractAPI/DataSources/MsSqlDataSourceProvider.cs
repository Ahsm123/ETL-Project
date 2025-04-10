using Dapper;
using ETL.Domain.Model;
using ETL.Domain.Rules;
using ETL.Domain.Sources;
using ETL.Domain.SQLQueryBuilder;
using ExtractAPI.Interfaces;
using Microsoft.Data.SqlClient;
using MySqlConnector;
using System.Text.Json;

namespace ExtractAPI.DataSources;

//public class MsSqlDataSourceProvider : IDataSourceProvider
//{
//    public bool CanHandle(Type sourceInfoType)
//        => typeof(MsSqlSourceInfo).IsAssignableFrom(sourceInfoType);

//    public async Task<JsonElement> GetDataAsync(ExtractConfig config)
//    {
//        if (config.SourceInfo is not MsSqlSourceInfo dbInfo)
//            throw new ArgumentException("Invalid sourceInfo: must be of type MsSqlSourceInfo");

//        if (string.IsNullOrWhiteSpace(dbInfo.ConnectionString))
//            throw new ArgumentException("Connection string is required");

//        var (query, parameters) = MsSqlQueryBuilder.BuildSafeSelectQuery(
//            dbInfo,
//            config.Fields,
//            config.Filters ?? new List<FilterRule>());

//        using var connection = new SqlConnection(dbInfo.ConnectionString);
//        await connection.OpenAsync();

//        var rows = await connection.QueryAsync(query, parameters);

//        var result = rows
//            .Select(row => ((IDictionary<string, object>)row)
//                .ToDictionary(kv => kv.Key, kv => kv.Value is DBNull ? null : kv.Value))
//            .ToList();

//        var json = JsonSerializer.Serialize(result);
//        using var doc = JsonDocument.Parse(json);
//        return doc.RootElement.Clone();
//    }
//}

