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

        public (string sql, DynamicParameters parameters) GenerateInsertQuery(DbTargetInfoBase targetInfo, Dictionary<string, object> rowData)
        {
            if (string.IsNullOrWhiteSpace(targetInfo.TargetTable))
                throw new ArgumentException("Target table is required");

            var escapedTableName = ProtectFromSqlInjection(targetInfo.TargetTable);

            var escapedColumnNames = rowData.Keys.Select(ProtectFromSqlInjection).ToList();
            var parameterPlaceholders = rowData.Keys.Select(columnName => $"@{columnName}").ToList();

            var insertQuery = $"INSERT INTO {escapedTableName} ({string.Join(", ", escapedColumnNames)}) VALUES ({string.Join(", ", parameterPlaceholders)});";

            var sqlParameters = new DynamicParameters();
            foreach (var (columnName, value) in rowData)
            {
                sqlParameters.Add($"@{columnName}", value ?? DBNull.Value);
            }

            return (insertQuery, sqlParameters);
        }

        public (string sql, DynamicParameters parameters) GenerateSelectQuery(DbSourceBaseInfo sourceInfo, List<string> selectedFields, List<FilterRule>? filterRules)
        {
            if (string.IsNullOrWhiteSpace(sourceInfo.TargetTable))
                throw new ArgumentException("Target table is required");

            var escapedTableName = ProtectFromSqlInjection(sourceInfo.TargetTable);

            var selectClause = (selectedFields != null && selectedFields.Any())
                ? string.Join(", ", selectedFields.Select(ProtectFromSqlInjection))
                : "*";

            var queryBuilder = new StringBuilder();
            var sqlParameters = new DynamicParameters();

            queryBuilder.Append($"SELECT {selectClause} FROM {escapedTableName}");

            if (filterRules != null && filterRules.Any())
            {
                var whereConditions = new List<string>();

                for (int index = 0; index < filterRules.Count; index++)
                {
                    var filter = filterRules[index];
                    var escapedColumn = ProtectFromSqlInjection(filter.Field);
                    var parameterName = $"@p{index}";

                    if (!AllowedOperators.TryGetValue(filter.Operator.ToLower(), out var sqlOperator))
                        throw new ArgumentException($"Unsupported operator '{filter.Operator}'");

                    whereConditions.Add($"{escapedColumn} {sqlOperator} {parameterName}");
                    sqlParameters.Add(parameterName, filter.Value);
                }

                queryBuilder.Append(" WHERE " + string.Join(" AND ", whereConditions));
            }

            queryBuilder.Append(";");

            return (queryBuilder.ToString(), sqlParameters);
        }




        private string ProtectFromSqlInjection(string identifier)
        {
            if (!Regex.IsMatch(identifier, @"^[a-zA-Z_][a-zA-Z0-9_]*$"))
                throw new ArgumentException($"Invalid identifier: {identifier}");

            return $"`{identifier}`"; // MySQL safe
        }
    }
}
