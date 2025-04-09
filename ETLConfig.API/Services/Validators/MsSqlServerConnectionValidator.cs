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
            var tableMetadata = new TableMetadata
            {
                TableName = table,
                Columns = (await connection.QueryAsync<string>(SqlQueryConstants.MSSQL_GetColumns, new { table })).ToList(),
                PrimaryKeys = (await connection.QueryAsync<string>(SqlQueryConstants.MSSQL_GetPrimaryKeys, new { table })).ToList(),
                ForeignKeys = (await connection.QueryAsync<ForeignKeyInfo>(SqlQueryConstants.MSSQL_GetForeignKeys, new { table })).ToList()
            };

            metadata.Tables.Add(tableMetadata);
        }

        return metadata;
    }
}
