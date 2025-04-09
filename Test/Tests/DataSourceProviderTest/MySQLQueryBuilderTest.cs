using ETL.Domain.Rules;
using ETL.Domain.Sources;
using ExtractAPI.DataSources.DatabaseQueryBuilder;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Tests.DataSourceProviderTest
{
    public class MySQLQueryBuilderTest
    {
        private readonly MySQLQueryBuilder _queryBuilder = new MySQLQueryBuilder();

        [Fact]
        public void BuildSelectQuery_ValidInput_ReturnsCorrectSql()
        {
            // Arrange
            var sourceInfo = new MySQLSourceInfo
            {
                Table = "users",
                Columns = new List<string> { "id", "name", "age" }
            };

            var filters = new List<FilterRule>
            {
                new FilterRule { Field = "age", Operator = "greater_than", Value = "30" },
                new FilterRule { Field = "name", Operator = "equals", Value = "John" }
            };

            // Act
            var (sql, parameters) = _queryBuilder.BuildSelectQuery(sourceInfo, filters);

            // Assert
            var expectedSql = "SELECT `id`, `name`, `age` FROM `users` WHERE `age` > @p0 AND `name` = @p1";
            Assert.Equal(expectedSql, sql);
            Assert.Equal("30", parameters.Get<string>("@p0"));
            Assert.Equal("John", parameters.Get<string>("@p1"));
        }
    }
}
