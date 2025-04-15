using Dapper;
using ETLConfig.API.Constants;
using ETLConfig.API.Models.DTOs;
using ETLConfig.API.Services.Interfaces;
using Microsoft.Data.SqlClient;

namespace ETLConfig.API.Services.Validators;

public class MsSqlConnectionValidator : IConnectionValidator
{
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
        var tableNames = (await connection.QueryAsync<string>(SqlQueryConstants.MSSQL_GetTables)).ToList();

        foreach (var table in tableNames)
        {
            var columns = (await connection.QueryAsync<ColumnMetadata>(
                SqlQueryConstants.MSSQL_GetColumns, new { table })).ToList();

            var primaryKeys = (await connection.QueryAsync<string>(
                SqlQueryConstants.MSSQL_GetPrimaryKeys, new { table })).ToList();

            var foreignKeys = (await connection.QueryAsync<ForeignKeyInfo>(
                SqlQueryConstants.MSSQL_GetForeignKeys, new { table })).ToList();

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
