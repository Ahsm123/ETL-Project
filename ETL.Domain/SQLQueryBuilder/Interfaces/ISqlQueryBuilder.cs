using Dapper;
using ETL.Domain.Rules;
using ETL.Domain.Sources;
using ETL.Domain.Targets.DbTargets;

namespace ExtractAPI.DataSources.DatabaseQueryBuilder.Interfaces;

public interface ISqlQueryBuilder
{
    (string sql, DynamicParameters parameters) GenerateSelectQuery(DbSourceBaseInfo info, List<string> fields, List<FilterRule> filters);
    (string sql, DynamicParameters parameters) GenerateInsertQuery(DbTargetInfoBase info, Dictionary<string, object> data);
}
