using Dapper;
using ETL.Domain.Rules;
using ETL.Domain.Sources;
using ETL.Domain.Targets.DbTargets;

namespace ExtractAPI.DataSources.DatabaseQueryBuilder.Interfaces
{
    public interface ISqlQueryBuilder
    {
        (string sql, DynamicParameters parameters) BuildSelectQuery(DbSourceBaseInfo info,List<string> fields, List<FilterRule> filters);
        (string sql, DynamicParameters parameters) BuildInsertQuery(DbTargetInfoBase info, Dictionary<string, object> data);
    }
}
