using Dapper;
using ETL.Domain.Rules;
using ETL.Domain.Sources;
using ETL.Domain.Sources.Db;
using ETL.Domain.SQLQueryBuilder.Interfaces;
using System.Text;

namespace ETL.Domain.SQLQueryBuilder;

public class MsSqlQueryBuilder : IMsSqlQueryBuilder
{
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
        string table = WrapIdentifier(info.TargetTable);
        string columns = fields?.Count > 0
            ? string.Join(", ", fields.Select(WrapIdentifier))
            : "*";

        string sql = $"SELECT {columns} FROM {table}";

        var (where, parameters) = BuildWhereClause(filters);
        if (!string.IsNullOrWhiteSpace(where))
            sql += $" WHERE {where}";

        return (sql, parameters);
    }

    public (string sql, DynamicParameters parameters) GenerateJoinedSelectQuery(JoinConfig config)
    {
        string baseTable = WrapIdentifier(config.BaseTable);
        string columns = config.Fields?.Count > 0
            ? string.Join(", ", config.Fields.Select(WrapIdentifier))
            : "*";

        var sb = new StringBuilder();
        sb.AppendLine($"SELECT {columns}");
        sb.AppendLine($"FROM {baseTable}");

        foreach (var join in config.Joins)
        {
            string joinTable = WrapIdentifier(join.Table);
            string joinType = join.JoinType.ToUpperInvariant();

            if (joinType is not ("INNER" or "LEFT" or "RIGHT" or "FULL"))
                throw new NotSupportedException($"Unsupported join type: {join.JoinType}");

            var onConditions = join.On.Select(j => $"{WrapIdentifier(j.Left)} = {WrapIdentifier(j.Right)}");
            sb.AppendLine($"{joinType} JOIN {joinTable} ON {string.Join(" AND ", onConditions)}");
        }

        var (where, parameters) = BuildWhereClause(config.Filters);
        if (!string.IsNullOrWhiteSpace(where))
            sb.AppendLine($"WHERE {where}");

        return (sb.ToString(), parameters);
    }

    public (string sql, DynamicParameters parameters) GenerateInsertQuery(string tableName, Dictionary<string, object> data)
    {
        if (data.Count == 0)
            throw new ArgumentException("No data provided for insert");

        string table = WrapIdentifier(tableName);
        var columns = data.Keys.Select(WrapIdentifier).ToList();
        var paramNames = data.Keys.Select(k => $"@{k}").ToList();

        var parameters = new DynamicParameters();
        foreach (var (key, value) in data)
            parameters.Add($"@{key}", value);

        string sql = $"INSERT INTO {table} ({string.Join(", ", columns)}) VALUES ({string.Join(", ", paramNames)})";
        return (sql, parameters);
    }

    private static (string clause, DynamicParameters parameters) BuildWhereClause(List<FilterRule>? filters)
    {
        var conditions = new List<string>();
        var parameters = new DynamicParameters();

        if (filters == null || filters.Count == 0)
            return (string.Empty, parameters);

        for (int i = 0; i < filters.Count; i++)
        {
            var f = filters[i];
            string op = OperatorMap.GetValueOrDefault(f.Operator.ToLower()) ?? throw new NotSupportedException($"Unsupported operator: {f.Operator}");
            string param = $"@param{i}";

            conditions.Add($"{WrapIdentifier(f.Field)} {op} {param}");
            parameters.Add(param, f.Value);
        }

        return (string.Join(" AND ", conditions), parameters);
    }

    private static string WrapIdentifier(string identifier)
    {
        // Supports dot notation like dbo.Table
        return string.Join('.', identifier.Split('.').Select(p => $"[{p}]"));
    }

    public bool ProtectFromSQLInjection(string query)
    {
        throw new NotImplementedException();
    }
}
