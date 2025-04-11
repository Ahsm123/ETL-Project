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
using ETL.Domain.Targets.DbTargets;

namespace Test.ExstractAPITest.DataSourceProviderTest
{
    public class MySQLQueryBuilderTest
    {
        [Fact]
        public void GenerateSelectQuery_GeneratesValidSqlAndParameters()
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

        [Fact]
        public void GenerateSelectQuery_GeneratesValidSQLWithoutFilter()
        {
            //arrange
            var queryBuilder = new MySQLQueryBuilder();
            var sourceInfo = new MySQLSourceInfo
            {
                TargetTable = "approved_highvalue_payments"
            };
            var fields = new List<string> { "account_id", "cost", "status" };
            //Act
            var (sql, parameters) = queryBuilder.GenerateSelectQuery(sourceInfo, fields, null);
            //Assert
            var expectedSql =
                "SELECT `account_id`, `cost`, `status` FROM `approved_highvalue_payments`;";
            Assert.Equal(expectedSql, sql.Trim());
        }
        [Fact]
        public void GenerateSelectQuery_GeneratesValidSQLWithEmptyFields()
        {
            //arrange
            var queryBuilder = new MySQLQueryBuilder();
            var sourceInfo = new MySQLSourceInfo
            {
                TargetTable = "approved_highvalue_payments"
            };
            var fields = new List<string>();
            //Act
            var (sql, parameters) = queryBuilder.GenerateSelectQuery(sourceInfo, fields, null);
            //Assert
            var expectedSql =
                "SELECT * FROM `approved_highvalue_payments`;";
            Assert.Equal(expectedSql, sql.Trim());
        }
        [Fact]
        public void GenerateSelectQuery_ThrowsExceptionWhenEmptyEmptyTargetTable()
        {
            //Arrange
            var queryBuilder = new MySQLQueryBuilder();
            var sourceInfo = new MySQLSourceInfo
            {
                TargetTable = ""
            };
            var fields = new List<string> { "account_id", "cost", "status" };
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                queryBuilder.GenerateSelectQuery(sourceInfo, fields, null));

            Assert.Equal("Target table is required", ex.Message);

        }
        [Fact]
        public void GenerateSelectQuery_WhenFilterRuleIsMissingAttribute_ThrowsException()
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
                new FilterRule { Field = "cost", Operator = "", Value = "1000" }
            };

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                queryBuilder.GenerateSelectQuery(sourceInfo, fields, filters));

            Assert.Equal("Unsupported operator ''", ex.Message);
        }
        [Fact]
        public void GenerateInsertQuery_ReturnsCorrectSqlAndParametersWhenValidInput()
        {
            // Arrange
            var queryBuilder = new MySQLQueryBuilder();
            var targetInfo = new MySqlTargetInfo
            {
                TargetTable = "approved_highvalue_payments"
            };
            var rowData = new Dictionary<string, object>
            {
                { "account_id", 123 },
                { "cost", 1500.50 },
                { "status", "approved" }
            };

            // Act
            var (sql, parameters) = queryBuilder.GenerateInsertQuery(targetInfo, rowData);

            // Assert
            var expectedSql =
                "INSERT INTO `approved_highvalue_payments` (`account_id`, `cost`, `status`) VALUES (@account_id, @cost, @status);";

            Assert.Equal(expectedSql, sql.Trim());
            Assert.Equal(3, parameters.ParameterNames.AsList().Count);
        }
        [Fact]
        public void GenerateInsertQuery_NullTargetTable_ThrowsArgumentException()
        {
            // Arrange
            var queryBuilder = new MySQLQueryBuilder();
            var targetInfo = new MySqlTargetInfo { TargetTable = null };
            var data = new Dictionary<string, object> { { "username", "king" } };

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                queryBuilder.GenerateInsertQuery(targetInfo, data));

            Assert.Equal("Target table is required", ex.Message);
        }
    }
}
