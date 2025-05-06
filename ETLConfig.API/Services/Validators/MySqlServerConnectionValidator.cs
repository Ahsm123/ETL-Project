using Dapper;
using ETLConfig.API.Models.DTOs;
using ETLConfig.API.Services.Interfaces;
using MySql.Data.MySqlClient;

namespace ETLConfig.API.Services.Validators;

public class MySqlServerConnectionValidator : IConnectionValidator
{
    private const string GetTablesQuery = "SHOW TABLES";
    private const string GetColumnsQuery = @"
    SELECT 
        COLUMN_NAME AS ColumnName,
        DATA_TYPE AS DataType,
        CASE WHEN IS_NULLABLE = 'YES' THEN TRUE ELSE FALSE END AS IsNullable,
        CHARACTER_MAXIMUM_LENGTH AS MaxLength,
        CASE WHEN EXTRA LIKE '%auto_increment%' THEN TRUE ELSE FALSE END AS IsAutoIncrement
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = @table
    AND TABLE_SCHEMA = DATABASE();";
    private const string GetPrimaryKeysQuery = "SHOW KEYS FROM `{0}` WHERE Key_name = 'PRIMARY'";
    private const string GetForeignKeysQuery = @"
SELECT 
    COLUMN_NAME AS `Column`,
    REFERENCED_TABLE_NAME AS ReferencedTable,
    REFERENCED_COLUMN_NAME AS ReferencedColumn
FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
WHERE TABLE_NAME = '{0}' AND REFERENCED_TABLE_NAME IS NOT NULL";


    public async Task<bool> IsValidAsync(string connectionString)
    {
        try
        {
            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<DatabaseMetadata> GetMetadataAsync(string connectionString)
    {
        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var metadata = new DatabaseMetadata();
        var tableNames = (await connection.QueryAsync<string>(GetTablesQuery)).ToList();

        foreach (var table in tableNames)
        {
            // Fetch column info
            var columns = (await connection.QueryAsync<ColumnMetadata>(
         GetColumnsQuery,
                new { table })).ToList();

            // Fetch primary keys
            var pkRaw = await connection.QueryAsync<dynamic>(
                string.Format(GetPrimaryKeysQuery, table));
            var primaryKeys = pkRaw.Select(row => (string)row.Column_name).ToList();

            // Fetch foreign keys
            var foreignKeys = (await connection.QueryAsync<ForeignKeyInfo>(
                string.Format(GetForeignKeysQuery, table))
            ).ToList();

            // Assemble table metadata
            var tableMetadata = new TableMetadata
            {
                TableName = table,
                Columns = columns,
                PrimaryKeys = primaryKeys,
                ForeignKeys = foreignKeys
            };

            metadata.Tables.Add(tableMetadata);
        }

        return metadata;
    }

    private int? ParseMaxLengthFromType(string type)
    {
        var start = type.IndexOf('(');
        var end = type.IndexOf(')');
        if (start != -1 && end > start)
        {
            var paramStr = type.Substring(start + 1, end - start - 1);
            var parts = paramStr.Split(',');
            if (int.TryParse(parts[0], out var len))
                return len;
        }

        return null;
    }
}
