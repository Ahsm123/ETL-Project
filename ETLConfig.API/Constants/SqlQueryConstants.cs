namespace ETLConfig.API.Constants;

public static class SqlQueryConstants
{
    // SQL Server Queries
    public const string MSSQL_GetTables = @"
        SELECT TABLE_NAME 
        FROM INFORMATION_SCHEMA.TABLES 
        WHERE TABLE_TYPE = 'BASE TABLE'";

    public const string MSSQL_GetColumns = @"
    SELECT 
        COLUMN_NAME AS ColumnName, 
        DATA_TYPE AS DataType, 
        CASE WHEN IS_NULLABLE = 'YES' THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS IsNullable,
        CHARACTER_MAXIMUM_LENGTH AS MaxLength,
        COLUMNPROPERTY(OBJECT_ID(TABLE_NAME), COLUMN_NAME, 'IsIdentity') AS IsAutoIncrement
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = @table";

    public const string MSSQL_GetPrimaryKeys = @"
        SELECT COLUMN_NAME 
        FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE 
        WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_NAME), 'IsPrimaryKey') = 1 
          AND TABLE_NAME = @table";

    public const string MSSQL_GetForeignKeys = @"
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


    // MySQL Queries
    public const string MYSQL_ShowTables = "SHOW TABLES";
    public const string MYSQL_ShowColumns = @"
    SELECT 
        COLUMN_NAME AS ColumnName,
        DATA_TYPE AS DataType,
        CASE WHEN IS_NULLABLE = 'YES' THEN TRUE ELSE FALSE END AS IsNullable,
        CHARACTER_MAXIMUM_LENGTH AS MaxLength,
        CASE WHEN EXTRA LIKE '%auto_increment%' THEN TRUE ELSE FALSE END AS IsAutoIncrement
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = @table
    AND TABLE_SCHEMA = DATABASE();";
    public const string MYSQL_ShowPrimaryKeys = "SHOW KEYS FROM `{0}` WHERE Key_name = 'PRIMARY'";
    public const string MYSQL_ShowForeignKeysAliased = @"
SELECT 
    COLUMN_NAME AS `Column`,
    REFERENCED_TABLE_NAME AS ReferencedTable,
    REFERENCED_COLUMN_NAME AS ReferencedColumn
FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
WHERE TABLE_NAME = '{0}' AND REFERENCED_TABLE_NAME IS NOT NULL";

}

