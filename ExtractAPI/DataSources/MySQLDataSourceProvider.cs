using ETL.Domain.Sources;
using MySqlConnector;
using System.Data;
using System.Text.Json;
using Dapper;
using ExtractAPI.DataSources.DatabaseQueryBuilder;
using ExtractAPI.DataSources.DatabaseQueryBuilder.Interfaces;
using ExtractAPI.Interfaces;
using ETL.Domain.Rules;
using ETL.Domain.Model;

namespace ExtractAPI.DataSources
{
    public class MySQLDataSourceProvider : IDataSourceProvider
    {
        private readonly ISqlQueryBuilder _queryBuilder;

        public MySQLDataSourceProvider(ISqlQueryBuilder queryBuilder)
        {
            _queryBuilder = queryBuilder;
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

            // ✅ Pass the fields list to the query builder
            
            var (query, parameters) = _queryBuilder.BuildSelectQuery(dbInfo,extractConfig.Fields,extractConfig.Filters);

            var result = new List<Dictionary<string, object>>();

            await using var connection = new MySqlConnection(dbInfo.ConnectionString);
            await connection.OpenAsync().ConfigureAwait(false);

            var rows = await connection.QueryAsync(query, parameters).ConfigureAwait(false);

            foreach (var row in rows)
            {
                var rowDict = new Dictionary<string, object>();

                foreach (var kv in (IDictionary<string, object>)row)
                {
                    rowDict[kv.Key] = kv.Value is DBNull ? null : kv.Value;
                }

                result.Add(rowDict);
            }

            var json = JsonSerializer.Serialize(result);
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.Clone();
        }
    }
    
}
