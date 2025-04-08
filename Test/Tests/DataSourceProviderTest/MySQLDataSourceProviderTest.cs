using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETL.Domain.Rules;
using ETL.Domain.Sources;
using ExtractAPI.DataSources.DatabaseQueryBuilder;
using ExtractAPI.DataSources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Tests.DataSourceProviderTest
{
    public class MySQLDataSourceProviderTest
    {
        [Fact]
        public async Task GetDataAsync_ReturnsUsersWithAgeGreaterThan30()
        {
            // Arrange
            var builder = new MySQLQueryBuilder();
            var provider = new MySQLDataSourceProvider(builder);

            var dbInfo = new MySQLSourceInfo
            {
                ConnectionString = "Server=localhost;Port=3306;Database=testdb;Uid=root;Pwd=root;",
                Table = "users",
                Columns = new List<string> { "id", "name", "email" },
                FilterRules = new List<FilterRule>
            {
                new FilterRule
                {
                    Field = "age",
                    Operator = "greater_than",
                    Value = "30"
                }
            },
                Provider = "mysql"
            };

            // Act
            var result = await provider.GetDataAsync(dbInfo);

            // Assert
            var json = result.ToString();
            Assert.Contains("Charlie Stone", json);  // age 35
            Assert.Contains("Eve Monroe", json);     // age 40
            Assert.DoesNotContain("Alice Smith", json); // age 30
            Assert.DoesNotContain("Bob Johnson", json); // age 25
        }
    }
}

