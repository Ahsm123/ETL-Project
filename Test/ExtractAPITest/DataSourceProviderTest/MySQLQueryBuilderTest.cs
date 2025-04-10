using ETL.Domain.Rules;
using ETL.Domain.Sources;
using ExtractAPI.DataSources.DatabaseQueryBuilder;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Test.ExstractAPITest.DataSourceProviderTest
{
    public class MySQLQueryBuilderTest
    {
        [Fact]
        public void BuildSelectQuery_GeneratesValidSqlAndParameters()
        {
            // Arrange
            var queryBuilder = new MySQLQueryBuilder();

            var sourceInfo = new MySQLSourceInfo
            {
                TargetTable = "approved_highvalue_payments"
            };

            var fields = new List<string> { "account_id", "cost", "status" };

            var filters = new List<FilterRule>
            {
                new FilterRule { Field = "cost", Operator = "greater_than", Value = "1000" },
                new FilterRule { Field = "status", Operator = "equals", Value = "approved" }
            };

            // Act
            var (sql, parameters) = queryBuilder.GenerateSelectQuery(sourceInfo, fields, filters);

            // Assert
            var expectedSql =
                "SELECT `account_id`, `cost`, `status` FROM `approved_highvalue_payments` WHERE `cost` > @p0 AND `status` = @p1;";

            Assert.Equal(expectedSql, sql.Trim());
            Assert.Equal(2, parameters.ParameterNames.AsList().Count);
            Assert.Equal("1000", parameters.Get<string>("@p0"));
            Assert.Equal("approved", parameters.Get<object>("@p1"));
        }
    }

}
