using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ETL.Domain.Rules;
using ETL.Domain.Sources;
using ExtractAPI.DataSources;
using ExtractAPI.DataSources.DatabaseQueryBuilder;
using Xunit;

namespace Test.Tests.DataSourceProviderTest
{
    public class MySQLDataSourceProviderTest
    {
        private const string ConnectionString = "Server=localhost;Port=3306;Database=mydb;Uid=user;Pwd=password;";
        private readonly MySQLDataSourceProvider _provider = new(new MySQLQueryBuilder());

        [Fact]
        public async Task GetDataAsync_FiltersUsersByAge()
        {
            var sourceInfo = new MySQLSourceInfo
            {
                ConnectionString = ConnectionString,
                Table = "users",
                Columns = new() { "id", "name", "age" }
            };

            var filters = new List<FilterRule>
            {
                new FilterRule { Field = "age", Operator = "greater_than", Value = "30" }
            };

            var result = await _provider.GetDataAsync(sourceInfo, filters);

            Assert.Equal(JsonValueKind.Array, result.ValueKind);

            var users = result.EnumerateArray().ToList();

            Assert.All(users, user =>
                Assert.True(user.GetProperty("age").GetInt32() > 30));

            Assert.Contains(users, user => user.GetProperty("name").GetString() == "Bob");
            Assert.Contains(users, user => user.GetProperty("name").GetString() == "Charlie");
        }
    }
}
