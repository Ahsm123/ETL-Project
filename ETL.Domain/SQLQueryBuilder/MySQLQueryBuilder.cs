using Dapper;
using ETL.Domain.Rules;
using ETL.Domain.Sources;
using ETL.Domain.Targets.DbTargets;
using ExtractAPI.DataSources.DatabaseQueryBuilder.Interfaces;
using System.Text;
using System.Text.RegularExpressions;

namespace ExtractAPI.DataSources.DatabaseQueryBuilder
{
    public class MySQLQueryBuilder : ISqlQueryBuilder
    {
        private static readonly Dictionary<string, string> AllowedOperators = new(StringComparer.OrdinalIgnoreCase)
        {
            ["equals"] = "=",
            ["not_equals"] = "!=",
            ["greater_than"] = ">",
            ["less_than"] = "<",
            ["greater_or_equal"] = ">=",
            ["less_or_equal"] = "<=",
        };

        public (string sql, DynamicParameters parameters) BuildInsertQuery(DbSourceBaseInfo info, Dictionary<string, object> data)
        {
            if (string.IsNullOrWhiteSpace(info.TargetTable))
                throw new ArgumentException("Target table is required");

            var tableName = SanitizeIdentifier(info.TargetTable);

            var columnNames = data.Keys.Select(SanitizeIdentifier).ToList();
            var paramNames = data.Keys.Select(k => $"@{k}").ToList();

            var sql = $"INSERT INTO {tableName} ({string.Join(", ", columnNames)}) VALUES ({string.Join(", ", paramNames)});";

            var parameters = new DynamicParameters();
            foreach (var kvp in data)
            {
                parameters.Add($"@{kvp.Key}", kvp.Value ?? DBNull.Value);
            }

            return (sql, parameters);

        }

        public (string sql, DynamicParameters parameters) BuildSelectQuery(DbSourceBaseInfo info,List<string> fields,List<FilterRule>? filters)
        {
            if (string.IsNullOrWhiteSpace(info.TargetTable))
                throw new ArgumentException("Target table is required");

            var tableName = SanitizeIdentifier(info.TargetTable);

            var selectColumns = (fields != null && fields.Any())
                ? string.Join(", ", fields.Select(SanitizeIdentifier))
                      : "*";

            var sqlBuilder = new StringBuilder();
            var parameters = new DynamicParameters();

            sqlBuilder.Append($"SELECT {selectColumns} FROM {tableName}");

            if (filters != null && filters.Any())
            {
                var whereClauses = new List<string>();

                for (int i = 0; i < filters.Count; i++)
                {
                    var rule = filters[i];
                    var column = SanitizeIdentifier(rule.Field);
                    var paramName = $"@p{i}";

                    if (!AllowedOperators.TryGetValue(rule.Operator.ToLower(), out var sqlOperator))
                        throw new ArgumentException($"Unsupported operator '{rule.Operator}'");

                    whereClauses.Add($"{column} {sqlOperator} {paramName}");
                    parameters.Add(paramName, rule.Value);
                }

                sqlBuilder.Append(" WHERE " + string.Join(" AND ", whereClauses));
            }

            sqlBuilder.Append(";");
            return (sqlBuilder.ToString(), parameters);
        }
       



        private string SanitizeIdentifier(string identifier)
        {
            if (!Regex.IsMatch(identifier, @"^[a-zA-Z_][a-zA-Z0-9_]*$"))
                throw new ArgumentException($"Invalid identifier: {identifier}");

            return $"`{identifier}`"; // MySQL safe
        }
    }
}
