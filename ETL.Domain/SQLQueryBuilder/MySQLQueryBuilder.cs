using Dapper;
using ETL.Domain.Rules;
using ETL.Domain.Sources;
using ETL.Domain.SQLQueryBuilder.Interfaces;
using ETL.Domain.Targets.DbTargets;
using ExtractAPI.DataSources.DatabaseQueryBuilder.Interfaces;
using System.Text;
using System.Text.RegularExpressions;

namespace ExtractAPI.DataSources.DatabaseQueryBuilder;

public class MySQLQueryBuilder : IMySqlQueryBuilder
{
    private static readonly Dictionary<string, string> AllowedOperators = new(StringComparer.OrdinalIgnoreCase)
    {
        ["equals"] = "=",
        ["not_equals"] = "!=",
        ["greater_than"] = ">",
        ["less_than"] = "<",
        ["greater_or_equal"] = ">=",
        ["less_or_equal"] = "<="
    };

    public (string sql, DynamicParameters parameters) GenerateInsertQuery(string tableName, Dictionary<string, object> rowData)
    {
        ValidateTableName(tableName);
        ValidateRowData(rowData);

        var escapedTable = EscapeIdentifier(tableName);
        var (columns, placeholders, parameters) = BuildInsertComponents(rowData);

        var sql = $"INSERT INTO {escapedTable} ({string.Join(", ", columns)}) VALUES ({string.Join(", ", placeholders)});";
        return (sql, parameters);
    }

    public (string sql, DynamicParameters parameters) GenerateSelectQuery(DbSourceBaseInfo sourceInfo, List<string> selectedFields, List<FilterRule>? filterRules)
    {
        ValidateTableName(sourceInfo.TargetTable);

        var tableName = EscapeIdentifier(sourceInfo.TargetTable);
        var fields = BuildFieldList(selectedFields);
        var baseQuery = new StringBuilder($"SELECT {fields} FROM {tableName}");

        var (whereClause, parameters) = BuildWhereClause(filterRules);

        if (!string.IsNullOrWhiteSpace(whereClause))
            baseQuery.Append(" WHERE ").Append(whereClause);

        baseQuery.Append(";");
        return (baseQuery.ToString(), parameters);
    }

    private static void ValidateTableName(string? tableName)
    {
        if (string.IsNullOrWhiteSpace(tableName))
            throw new ArgumentException("Target table is required.");
    }

    private static void ValidateRowData(Dictionary<string, object> rowData)
    {
        if (rowData == null || rowData.Count == 0)
            throw new ArgumentException("No data provided for insert.");
    }

    private static string BuildFieldList(List<string> fields)
    {
        if (fields == null || fields.Count == 0)
            return "*";

        return string.Join(", ", fields.Select(EscapeIdentifier));
    }

    private static (List<string> columns, List<string> paramPlaceholders, DynamicParameters parameters) BuildInsertComponents(Dictionary<string, object> rowData)
    {
        var columns = new List<string>();
        var placeholders = new List<string>();
        var parameters = new DynamicParameters();

        foreach (var (column, value) in rowData)
        {
            var escapedColumn = EscapeIdentifier(column);
            var paramName = $"@{column}";

            columns.Add(escapedColumn);
            placeholders.Add(paramName);
            parameters.Add(paramName, value ?? DBNull.Value);
        }

        return (columns, placeholders, parameters);
    }

    private static (string clause, DynamicParameters parameters) BuildWhereClause(List<FilterRule>? filterRules)
    {
        var conditions = new List<string>();
        var parameters = new DynamicParameters();

        if (filterRules == null || filterRules.Count == 0)
            return (string.Empty, parameters);

        for (int i = 0; i < filterRules.Count; i++)
        {
            var rule = filterRules[i];
            var column = EscapeIdentifier(rule.Field);
            var paramName = $"@p{i}";

            if (!AllowedOperators.TryGetValue(rule.Operator.ToLower(), out var sqlOperator))
                throw new ArgumentException($"Unsupported operator '{rule.Operator}'");

            conditions.Add($"{column} {sqlOperator} {paramName}");
            parameters.Add(paramName, rule.Value);
        }

        return (string.Join(" AND ", conditions), parameters);
    }

    private static string EscapeIdentifier(string identifier)
    {
        if (!Regex.IsMatch(identifier, @"^[a-zA-Z_][a-zA-Z0-9_]*$"))
            throw new ArgumentException($"Invalid identifier: {identifier}");

        return $"`{identifier}`"; // MySQL-safe
    }

    public bool ProtectFromSQLInjection(string query)
    {
        throw new NotImplementedException();
    }
}
