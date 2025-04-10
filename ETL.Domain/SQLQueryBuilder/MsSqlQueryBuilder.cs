using Dapper;
using ETL.Domain.Rules;
using ETL.Domain.Sources;
using ETL.Domain.Targets.DbTargets;
using ExtractAPI.DataSources.DatabaseQueryBuilder.Interfaces;
using System.Text.RegularExpressions;

namespace ETL.Domain.SQLQueryBuilder;

public class MsSqlQueryBuilder : ISqlQueryBuilder
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

        var tableName = ProtectFromSqlInjection(info.TargetTable);

        var selectColumns = (fields != null && fields.Any())
            ? string.Join(", ", fields.Select(ProtectFromSqlInjection))
            : "*";

        var baseQuery = string.Format(BaseSelectQuery, selectColumns, tableName);

        var (whereClause, parameters) = GenerateWhereCondition(filters);

        var finalQuery = string.IsNullOrWhiteSpace(whereClause)
            ? baseQuery
            : $"{baseQuery} WHERE {whereClause}";

        return (finalQuery, parameters);
    }

    private static (string clause, DynamicParameters parameters) GenerateWhereCondition(List<FilterRule>? filters)
    {
        var clauseParts = new List<string>();
        var parameters = new DynamicParameters();

        if (filters == null || filters.Count == 0)
            return (string.Empty, parameters);

        for (int i = 0; i < filters.Count; i++)
        {
            var rule = filters[i];
            var paramName = $"@param{i}";

            if (!OperatorMap.TryGetValue(rule.Operator.ToLower(), out var sqlOperator))
                throw new NotSupportedException($"Unsupported operator: {rule.Operator}");

            clauseParts.Add($"{ProtectFromSqlInjection(rule.Field)} {sqlOperator} {paramName}");
            parameters.Add(paramName, rule.Value);
        }

        return (string.Join(" AND ", clauseParts), parameters);
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

