using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.Domain.SQLQueryBuilder.Interfaces;

public interface ISqlExecutor
{
    Task<IEnumerable<IDictionary<string, object>>> ExecuteQueryAsync(string connectionString, string query, object parameters);
}
