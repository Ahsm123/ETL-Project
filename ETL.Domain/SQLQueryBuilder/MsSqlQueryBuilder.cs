using Dapper;
using ETL.Domain.Rules;
using ETL.Domain.Sources;
using ETL.Domain.SQLQueryBuilder.Interfaces;
using ETL.Domain.Targets.DbTargets;
using ExtractAPI.DataSources.DatabaseQueryBuilder.Interfaces;
using System.Text.RegularExpressions;

namespace ETL.Domain.SQLQueryBuilder;

public class MsSqlQueryBuilder : IMsSqlQueryBuilder
{
    private const string BaseSelectQuery = "SELECT {0} FROM {1}";

    private static readonly Dictionary<string, string> OperatorMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["equals"] = "=",
        ["not_equals"] = "!=",
        ["greater_than"] = ">",
        ["less_than"] = "<",
        ["greater_or_equal"] = ">=",
        ["less_or_equal"] = "<=",
    };

    public (string sql, DynamicParameters parameters) GenerateSelectQuery(DbSourceBaseInfo info, List<string> fields, List<FilterRule>? filters)
    {
        if (string.IsNullOrWhiteSpace(info.TargetTable))
            throw new ArgumentException("Target table is required");

        var sanitizedTableName = ProtectFromSqlInjection(info.TargetTable);

        var columnSelection = (fields != null && fields.Any())
            ? string.Join(", ", fields.Select(ProtectFromSqlInjection))
            : "*";

        var selectStatement = string.Format(BaseSelectQuery, columnSelection, sanitizedTableName);

        var (whereClause, whereParameters) = GenerateWhereCondition(filters);

        var completeQuery = string.IsNullOrWhiteSpace(whereClause)
            ? selectStatement
            : $"{selectStatement} WHERE {whereClause}";

        return (completeQuery, whereParameters);
    }

    private static (string clause, DynamicParameters parameters) GenerateWhereCondition(List<FilterRule>? filters)
    {
        var whereConditions = new List<string>();
        var dynamicParams = new DynamicParameters();

        if (filters == null || filters.Count == 0)
            return (string.Empty, dynamicParams);

        for (int index = 0; index < filters.Count; index++)
        {
            var filter = filters[index];
            var parameterName = $"@param{index}";

            if (!OperatorMap.TryGetValue(filter.Operator.ToLower(), out var sqlOperator))
                throw new NotSupportedException($"Unsupported operator: {filter.Operator}");

            var escapedFieldName = ProtectFromSqlInjection(filter.Field);
            whereConditions.Add($"{escapedFieldName} {sqlOperator} {parameterName}");
            dynamicParams.Add(parameterName, filter.Value);
        }

        return (string.Join(" AND ", whereConditions), dynamicParams);
    }

    private static string ProtectFromSqlInjection(string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
            throw new ArgumentException("Field/table name cannot be empty.");

        if (!Regex.IsMatch(identifier, "^[a-zA-Z_][a-zA-Z0-9_]*$"))
            throw new ArgumentException($"Invalid characters in identifier: {identifier}");

        return $"[{identifier}]"; 
    }

    public (string sql, DynamicParameters parameters) GenerateInsertQuery(DbTargetInfoBase info, Dictionary<string, object> data)
    {
        throw new NotImplementedException();
    }
}

