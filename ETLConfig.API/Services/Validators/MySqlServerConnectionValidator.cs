using Dapper;
using ETLConfig.API.Constants;
using ETLConfig.API.Models.DTOs;
using ETLConfig.API.Services.Interfaces;
using MySql.Data.MySqlClient;

namespace ETLConfig.API.Services.Validators;

public class MySqlServerConnectionValidator : IConnectionValidator
{
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
        var tableNames = (await connection.QueryAsync<string>(SqlQueryConstants.MYSQL_ShowTables)).ToList();

        foreach (var table in tableNames)
        {
            // Fetch column info
            var columns = (await connection.QueryAsync<ColumnMetadata>(
    SqlQueryConstants.MYSQL_ShowColumns,
    new { table })).ToList();


            // Fetch primary keys
            var pkRaw = await connection.QueryAsync<dynamic>(
                string.Format(SqlQueryConstants.MYSQL_ShowPrimaryKeys, table));
            var primaryKeys = pkRaw.Select(row => (string)row.Column_name).ToList();

            // Fetch foreign keys
            var foreignKeys = (await connection.QueryAsync<ForeignKeyInfo>(
                string.Format(SqlQueryConstants.MYSQL_ShowForeignKeysAliased, table))
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
        // Example type: varchar(255), decimal(10,2)
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
