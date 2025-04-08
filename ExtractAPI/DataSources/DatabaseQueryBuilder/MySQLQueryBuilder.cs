using Dapper;
using ETL.Domain.Sources;
using ExtractAPI.DataSources.DatabaseQueryBuilder.Interfaces;
using MySqlConnector.Logging;
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

        public (string sql, DynamicParameters parameters) BuildSelectQuery(MySQLSourceInfo info)
        {
            if (string.IsNullOrWhiteSpace(info.Table))
                throw new ArgumentException("Table is required");

            var tableName = SanitizeIdentifier(info.Table);

            // If no columns are specified, default to all
            var selectColumns = (info.Columns != null && info.Columns.Any())
                ? string.Join(", ", info.Columns.Select(SanitizeIdentifier))
                : "*";

            var sb = new StringBuilder();
            var parameters = new DynamicParameters();

            sb.Append($"SELECT {selectColumns} FROM {tableName}");

            if (info.FilterRules != null && info.FilterRules.Any())
            {
                var whereClauses = new List<string>();

                for (int i = 0; i < info.FilterRules.Count; i++)
                {
                    var rule = info.FilterRules[i];

                    var column = SanitizeIdentifier(rule.Field);
                    var paramName = $"@p{i}";

                    if (!AllowedOperators.TryGetValue(rule.Operator, out var sqlOperator))
                        throw new ArgumentException($"Unsupported operator '{rule.Operator}'");

                    whereClauses.Add($"{column} {sqlOperator} {paramName}");
                    parameters.Add(paramName, rule.Value);
                }

                sb.Append(" WHERE " + string.Join(" AND ", whereClauses));
            }

            var finalSql = sb.ToString();
            return (finalSql, parameters);
        }
    
        

        private string SanitizeIdentifier(string identifier)
        {
            if (!Regex.IsMatch(identifier, @"^[a-zA-Z_][a-zA-Z0-9_]*$"))
                throw new ArgumentException($"Invalid identifier: {identifier}");

            return $"`{identifier}`"; // MySQL safe
        }
    }
}
