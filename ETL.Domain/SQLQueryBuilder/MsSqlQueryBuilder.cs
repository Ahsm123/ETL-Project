using Dapper;
using ETL.Domain.Rules;
using ETL.Domain.Sources;
using ETL.Domain.SQLQueryBuilder.Interfaces;
using ETL.Domain.Targets.DbTargets;
using System.Text.RegularExpressions;

namespace ETL.Domain.SQLQueryBuilder;

public class MsSqlQueryBuilder : IMsSqlQueryBuilder
{
    private const string BaseSelectTemplate = "SELECT {0} FROM {1}";

    private static readonly Dictionary<string, string> OperatorMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["equals"] = "=",
        ["not_equals"] = "!=",
        ["greater_than"] = ">",
        ["less_than"] = "<",
        ["greater_or_equal"] = ">=",
        ["less_or_equal"] = "<="
    };

    public (string sql, DynamicParameters parameters) GenerateSelectQuery(DbSourceBaseInfo info, List<string>? fields, List<FilterRule>? filters)
    {
        throw new NotImplementedException();
        //ValidateTableName(sourceInfo.TargetTable);

        //string table = FormatIdentifier(sourceInfo.TargetTable);
        //string columns = fields?.Any() == true
        //    ? string.Join(", ", fields.Select(FormatIdentifier))
        //    : "*";

        //string baseQuery = string.Format(BaseSelectTemplate, columns, table);
        //var (whereClause, parameters) = BuildWhereClause(filters);

        //string completeQuery = string.IsNullOrWhiteSpace(whereClause)
        //    ? baseQuery
        //    : $"{baseQuery} WHERE {whereClause}";

        //return (completeQuery, parameters);
    }

    public (string sql, DynamicParameters parameters) GenerateInsertQuery(string tableName, Dictionary<string, object> data)
    {
        throw new NotImplementedException();
        //if (targetInfo is not MsSqlTargetInfo sqlTarget)
        //    throw new ArgumentException("Invalid target info type");

        //ValidateTableName(sqlTarget.TargetTable);
        //if (data == null || !data.Any())
        //    throw new ArgumentException("No data provided for insert");

        //string table = FormatIdentifier(sqlTarget.TargetTable);
        //var (columns, paramNames, parameters) = BuildInsertComponents(data);

        //string insertQuery = $"INSERT INTO {table} ({string.Join(", ", columns)}) VALUES ({string.Join(", ", paramNames)})";
        //return (insertQuery, parameters);
    }

    private static (List<string> columns, List<string> paramNames, DynamicParameters parameters) BuildInsertComponents(Dictionary<string, object> data)
    {
        var columns = new List<string>();
        var paramNames = new List<string>();
        var parameters = new DynamicParameters();

        foreach (var (key, value) in data)
        {
            string column = FormatIdentifier(key);
            string paramName = $"@{key}";

            columns.Add(column);
            paramNames.Add(paramName);
            parameters.Add(paramName, value);
        }

        return (columns, paramNames, parameters);
    }

    private static (string clause, DynamicParameters parameters) BuildWhereClause(List<FilterRule>? filters)
    {
        var conditions = new List<string>();
        var parameters = new DynamicParameters();

        if (filters == null || filters.Count == 0)
            return (string.Empty, parameters);

        for (int i = 0; i < filters.Count; i++)
        {
            var rule = filters[i];
            string paramName = $"@param{i}";

            if (!OperatorMap.TryGetValue(rule.Operator.ToLower(), out var sqlOperator))
                throw new NotSupportedException($"Unsupported operator: {rule.Operator}");

            string column = FormatIdentifier(rule.Field);
            conditions.Add($"{column} {sqlOperator} {paramName}");
            parameters.Add(paramName, rule.Value);
        }

        return (string.Join(" AND ", conditions), parameters);
    }

    private static string FormatIdentifier(string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
            throw new ArgumentException("Field/table name cannot be empty.");

        if (!Regex.IsMatch(identifier, "^[a-zA-Z_][a-zA-Z0-9_]*$"))
            throw new ArgumentException($"Invalid characters in identifier: {identifier}");

        return $"[{identifier}]";
    }

    private static void ValidateTableName(string? tableName)
    {
        if (string.IsNullOrWhiteSpace(tableName))
            throw new ArgumentException("Target table is required.");
    }
}
