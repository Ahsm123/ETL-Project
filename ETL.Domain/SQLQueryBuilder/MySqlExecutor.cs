using Dapper;
using ETL.Domain.SQLQueryBuilder.Interfaces;
using MySqlConnector;

namespace ETL.Domain.SQLQueryBuilder;

public class MySqlExecutor : IMySqlExecutor
{
    public async Task<IEnumerable<IDictionary<string, object>>> ExecuteQueryAsync(string connectionString, string query, object parameters)
    {
        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync().ConfigureAwait(false);

        var dapperParams = new DynamicParameters();
        if (parameters is Dictionary<string, object> dict)
        {
            foreach (var kv in dict)
                dapperParams.Add(kv.Key, kv.Value);
        }
        else
        {
            dapperParams = parameters as DynamicParameters ?? new DynamicParameters();
        }

        var rows = await connection.QueryAsync(query, dapperParams).ConfigureAwait(false);
        return rows.Select(r => (IDictionary<string, object>)r).ToList();
    }

    public async Task<object?> ExecuteInsertWithIdentityAsync(string connectionString, string query, object parameters)
    {
        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync().ConfigureAwait(false);

        var dapperParams = new DynamicParameters();
        if (parameters is Dictionary<string, object> dict)
        {
            foreach (var kv in dict)
                dapperParams.Add(kv.Key, kv.Value);
        }
        else
        {
            dapperParams = parameters as DynamicParameters ?? new DynamicParameters();
        }

        var fullQuery = $"{query}; SELECT LAST_INSERT_ID();";
        var result = await connection.ExecuteScalarAsync(fullQuery, dapperParams).ConfigureAwait(false);

        return result;
    }

}
