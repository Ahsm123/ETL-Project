using Dapper;
using ETL.Domain.Sources;

namespace ExtractAPI.DataSources.DatabaseQueryBuilder.Interfaces
{
    public interface ISqlQueryBuilder
    {
        (string sql, DynamicParameters parameters) BuildSelectQuery(MySQLSourceInfo info);
    }
}
