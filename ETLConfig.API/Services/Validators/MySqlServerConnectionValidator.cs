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
            var columnsRaw = await connection.QueryAsync<dynamic>(
                string.Format(SqlQueryConstants.MYSQL_ShowColumns, table));
            var columns = columnsRaw.Select(row => (string)row.Field).ToList();

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


    //public async Task<DatabaseMetadata> GetMetadataAsync(string connectionString)
    //{
    //    await using var connection = new MySqlConnection(connectionString);
    //    await connection.OpenAsync();

    //    var metadata = new DatabaseMetadata();
    //    var tableNames = (await connection.QueryAsync<string>(SqlQueryConstants.MYSQL_ShowTables)).ToList();

    //    foreach (var table in tableNames)
    //    {
    //        var tableMetadata = new TableMetadata
    //        {
    //            TableName = table,
    //            Columns = (await connection.QueryAsync<string>(
    //                $"SELECT `Field` FROM ({string.Format(SqlQueryConstants.MYSQL_ShowColumns, table)}) AS t")).ToList(),

    //            PrimaryKeys = (await connection.QueryAsync<string>(
    //                $"SELECT `Column_name` FROM ({string.Format(SqlQueryConstants.MYSQL_ShowPrimaryKeys, table)}) AS t")).ToList(),

    //            ForeignKeys = (await connection.QueryAsync<ForeignKeyInfo>(
    //                string.Format(SqlQueryConstants.MYSQL_ShowForeignKeysAliased, table))
    //            ).ToList()
    //        };

    //        metadata.Tables.Add(tableMetadata);
    //    }

    //    return metadata;
    //}
}
