using ETL.Domain.Rules;
using ETL.Domain.Sources;

namespace ETL.Domain.SQLQueryBuilder;

public static class MsSqlQueryBuilder
{
    private static readonly Dictionary<string, string> _operatorMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["equals"] = "=",
        ["not_equals"] = "!=",
        ["greater_than"] = ">",
        ["less_than"] = "<",
        ["greater_or_equal"] = ">=",
        ["less_or_equal"] = "<=",
    };

    public static (string query, Dictionary<string, object> parameters) BuildSafeSelectQuery(
        DbSourceBaseInfo source,
        List<string> fields,
        List<FilterRule> filters)
    {
        string fieldList = BuildFieldList(fields);
        string fromClause = SanitizeIdentifier(source.TargetTable);
        string baseQuery = $"SELECT {fieldList} FROM {fromClause}";

        var (whereClause, parameters) = BuildWhereClause(filters);

        string finalQuery = string.IsNullOrWhiteSpace(whereClause)
            ? baseQuery
            : $"{baseQuery} WHERE {whereClause}";

        return (finalQuery, parameters);
    }

    private static string BuildFieldList(List<string> fields)
    {
        if (fields == null || fields.Count == 0)
            return "*";

        return string.Join(", ", fields.Select(SanitizeIdentifier));
    }

    private static (string clause, Dictionary<string, object> parameters) BuildWhereClause(List<FilterRule> filters)
    {
        var clauseParts = new List<string>();
        var parameters = new Dictionary<string, object>();

        if (filters == null || filters.Count == 0)
            return (string.Empty, parameters);

        for (int i = 0; i < filters.Count; i++)
        {
            var rule = filters[i];
            var paramName = $"@param{i}";

            if (!_operatorMap.TryGetValue(rule.Operator.ToLower(), out var sqlOperator))
                throw new NotSupportedException($"Unsupported operator: {rule.Operator}");

            clauseParts.Add($"{SanitizeIdentifier(rule.Field)} {sqlOperator} {paramName}");
            parameters[paramName] = rule.Value;
        }

        return (string.Join(" AND ", clauseParts), parameters);
    }

    private static string SanitizeIdentifier(string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
            throw new ArgumentException("Field/table name cannot be empty.");

        if (identifier.Any(c => !char.IsLetterOrDigit(c) && c != '_'))
            throw new ArgumentException($"Invalid characters in identifier: {identifier}");

        return $"[{identifier}]";
    }
}
