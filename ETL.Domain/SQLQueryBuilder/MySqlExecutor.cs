using Dapper;
using ETL.Domain.SQLQueryBuilder.Interfaces;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.Domain.SQLQueryBuilder;

public class MySqlExecutor : ISqlExecutor
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
}
