using Dapper;
using ETL.Domain.SQLQueryBuilder.Interfaces;
using ETL.Domain.Targets;
using ETL.Domain.Targets.DbTargets;
using ExtractAPI.DataSources.DatabaseQueryBuilder.Interfaces;
using Load.Interfaces;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Load.Writers
{
    public class MySqlTargetWriter : ITargetWriter
    {
        private readonly IMySqlQueryBuilder _queryBuilder;

        public MySqlTargetWriter(IMySqlQueryBuilder queryBuilder)
        {
            _queryBuilder = queryBuilder;
        }

        public bool CanHandle(Type targetInfoType)
        
            => typeof(MySqlTargetInfo).IsAssignableFrom(targetInfoType);
            

        public async Task WriteAsync(TargetInfoBase targetInfo, Dictionary<string, object> data, string? pipelineId = null)
        {
            if (targetInfo is not MySqlTargetInfo info)
                throw new ArgumentException("Invalid target info type");

            var (sql, parameters) = _queryBuilder.GenerateInsertQuery(info, data);
            try
            {

                await using var connection = new MySqlConnection(BuildConnectionString(info));
                await connection.OpenAsync();

                await connection.ExecuteAsync(sql, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to write to MySQL target: {ex.Message}", ex);
            }
        }

        private string BuildConnectionString(MySqlTargetInfo info)
        {
            var builder = new MySqlConnectionStringBuilder(info.ConnectionString)
            {
                SslMode = info.UseSsl ? MySqlSslMode.Required : MySqlSslMode.None
            };

            return builder.ConnectionString;
        }
    }
}
