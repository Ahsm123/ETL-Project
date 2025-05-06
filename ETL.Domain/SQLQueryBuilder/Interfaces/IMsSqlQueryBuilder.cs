using Dapper;
using ETL.Domain.Sources.Db;
using ExtractAPI.DataSources.DatabaseQueryBuilder.Interfaces;

namespace ETL.Domain.SQLQueryBuilder.Interfaces;

public interface IMsSqlQueryBuilder : ISqlQueryBuilder
{
    (string sql, DynamicParameters parameters) GenerateJoinedSelectQuery(JoinConfig config);
}
