using Dapper;
using ETL.Domain.Rules;
using ETL.Domain.Sources;

namespace ExtractAPI.DataSources.DatabaseQueryBuilder.Interfaces
{
    public interface ISqlQueryBuilder
    {
        (string sql, DynamicParameters parameters) BuildSelectQuery(MySQLSourceInfo info, List<FilterRule> filters);
    }
}
