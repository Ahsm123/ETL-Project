﻿using Dapper;
using ETL.Domain.SQLQueryBuilder.Interfaces;
using Microsoft.Data.SqlClient;

namespace ETL.Domain.SQLQueryBuilder;

public class MsSqlExecutor : IMsSqlExecutor
{
    public Task<object?> ExecuteInsertWithIdentityAsync(string connectionString, string query, object parameters)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<IDictionary<string, object>>> ExecuteQueryAsync(
        string connectionString,
        string query,
        object parameters)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var rows = await connection.QueryAsync(query, parameters);

        return rows
            .Select(row => (IDictionary<string, object>)row)
            .ToList();
    }
}
