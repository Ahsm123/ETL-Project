﻿using ETL.Domain.Sources;
using MySqlConnector;
using System.Data;
using System.Text.Json;
using Dapper;
using ExtractAPI.DataSources.DatabaseQueryBuilder;
using ExtractAPI.DataSources.DatabaseQueryBuilder.Interfaces;
using ETL.Domain.Rules;
using ETL.Domain.Model;
using ETL.Domain.SQLQueryBuilder.Interfaces;
using ETL.Domain.SQLQueryBuilder;
using ExtractAPI.SourceProviders.Interfaces;

namespace ExtractAPI.DataSources
{
    public class MySQLDataSourceProvider : IDataSourceProvider
    {
        private readonly IMySqlQueryBuilder _queryBuilder;
        private readonly IMySqlExecutor _sqlExecutor;

        public MySQLDataSourceProvider(IMySqlQueryBuilder queryBuilder, IMySqlExecutor sqlExecutor)
        {
            _queryBuilder = queryBuilder;
            _sqlExecutor = sqlExecutor;
        }

        public bool CanHandle(Type sourceInfoType)
        {
            return sourceInfoType == typeof(MySQLSourceInfo);
        }

        public async Task<JsonElement> GetDataAsync(ExtractConfig extractConfig)
        {
            if (extractConfig.SourceInfo is not MySQLSourceInfo dbInfo)
                throw new ArgumentException("Invalid sourceInfo: must be of type MySQLSourceInfo");

            if (string.IsNullOrWhiteSpace(dbInfo.ConnectionString))
                throw new ArgumentException("Connection string is required");

            var (query, parameters) = _queryBuilder.GenerateSelectQuery(dbInfo, extractConfig.Fields, extractConfig.Filters);

            var rows = await _sqlExecutor.ExecuteQueryAsync(dbInfo.ConnectionString, query, parameters);

            var json = JsonSerializer.Serialize(rows);
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.Clone();
        }
    }
    
}
