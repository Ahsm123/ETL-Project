using Dapper;
using ETLConfig.API.Models.DTOs;
using ETLConfig.API.Services.Interfaces;
using Microsoft.Data.SqlClient;

namespace ETLConfig.API.Services.Validators;

public class MsSqlConnectionValidator : IConnectionValidator
{
    private const string GetTablesQuery = @"
        SELECT TABLE_NAME 
        FROM INFORMATION_SCHEMA.TABLES 
        WHERE TABLE_TYPE = 'BASE TABLE'";
    private const string GetColumsQuery = @"
        SELECT 
        COLUMN_NAME AS ColumnName, 
        DATA_TYPE AS DataType, 
        CASE WHEN IS_NULLABLE = 'YES' THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS IsNullable,
        CHARACTER_MAXIMUM_LENGTH AS MaxLength,
        COLUMNPROPERTY(OBJECT_ID(TABLE_NAME), COLUMN_NAME, 'IsIdentity') AS IsAutoIncrement
        FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = @table";
    private const string GetPrimaryKeysQuery = @"
        SELECT COLUMN_NAME 
        FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE 
        WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_NAME), 'IsPrimaryKey') = 1 
          AND TABLE_NAME = @table";
    private const string GetForeignKeysQuery = @"
        SELECT 
    fkc.COLUMN_NAME AS [Column],
    pk.TABLE_NAME AS ReferencedTable,
    pkc.COLUMN_NAME AS ReferencedColumn
FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS rc
JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE fkc 
    ON rc.CONSTRAINT_NAME = fkc.CONSTRAINT_NAME
JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE pkc 
    ON rc.UNIQUE_CONSTRAINT_NAME = pkc.CONSTRAINT_NAME
JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS pk 
    ON pk.CONSTRAINT_NAME = rc.UNIQUE_CONSTRAINT_NAME
WHERE fkc.TABLE_NAME = @table";
    public async Task<bool> IsValidAsync(string connectionString)
    {
        try
        {
            using var connection = new SqlConnection(connectionString);
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
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var metadata = new DatabaseMetadata();
        var tableNames = (await connection.QueryAsync<string>(GetTablesQuery)).ToList();

        foreach (var table in tableNames)
        {
            var columns = (await connection.QueryAsync<ColumnMetadata>(
                GetColumsQuery, new { table })).ToList();

            var primaryKeys = (await connection.QueryAsync<string>(
                GetPrimaryKeysQuery, new { table })).ToList();

            var foreignKeys = (await connection.QueryAsync<ForeignKeyInfo>(
                GetForeignKeysQuery, new { table })).ToList();

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
}
