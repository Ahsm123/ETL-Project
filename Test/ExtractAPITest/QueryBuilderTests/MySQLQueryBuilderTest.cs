using Dapper;
using ETL.Domain.Rules;
using ETL.Domain.Targets.DbTargets;
using ExtractAPI.DataSources.DatabaseQueryBuilder;
using Xunit;

namespace Test.ExtractAPITest.QueryBuilderTests
{
    public class MySQLQueryBuilderTest
    {
        private static MySqlTargetInfo GetStandardTargetInfo()
        {
            return new MySqlTargetInfo
            {
                ConnectionString = "Server=localhost;Database=testdb;Uid=root;Pwd=password;",
                LoadMode = "append"
            };
        }

        private static TargetTableConfig GetStandardTable()
        {
            return new TargetTableConfig
            {
                TargetTable = "approved_highvalue_payments",
                Fields = new List<LoadFieldMapRule>
                {
                    new LoadFieldMapRule { SourceField = "account_id", TargetField = "account_id" },
                    new LoadFieldMapRule { SourceField = "cost", TargetField = "cost" },
                    new LoadFieldMapRule { SourceField = "status", TargetField = "status" }
                }
            };
        }

        [Fact]
        public void GenerateInsertQuery_ValidTargetInfo_GeneratesCorrectSQL()
        {
            var queryBuilder = new MySQLQueryBuilder();
            var table = GetStandardTable();

            var rowData = new Dictionary<string, object>
            {
                { "account_id", 123 },
                { "cost", 1500.50 },
                { "status", "approved" }
            };

            var (sql, parameters) = queryBuilder.GenerateInsertQuery(table.TargetTable, rowData);

            var expectedSql = "INSERT INTO `approved_highvalue_payments` (`account_id`, `cost`, `status`) VALUES (@account_id, @cost, @status);";
            Assert.Equal(expectedSql, sql.Trim());
            Assert.Equal(3, parameters.ParameterNames.Count());
            Assert.Equal(123, parameters.Get<int>("@account_id"));
            Assert.Equal(1500.50, parameters.Get<double>("@cost"));
            Assert.Equal("approved", parameters.Get<string>("@status"));
        }

        [Fact]
        public void GenerateInsertQuery_WithNullValue_UsesDBNull()
        {
            var queryBuilder = new MySQLQueryBuilder();
            var rowData = new Dictionary<string, object> { { "message", null } };

            var (sql, parameters) = queryBuilder.GenerateInsertQuery("logs", rowData);

            Assert.Equal("INSERT INTO `logs` (`message`) VALUES (@message);", sql.Trim());
            Assert.Null(parameters.Get<object>("@message"));
        }

        [Fact]
        public void GenerateInsertQuery_WithInvalidTableName_ThrowsException()
        {
            var queryBuilder = new MySQLQueryBuilder();
            var rowData = new Dictionary<string, object> { { "username", "king" } };

            var ex = Assert.Throws<ArgumentException>(() =>
                queryBuilder.GenerateInsertQuery("users; DROP TABLE users;", rowData));

            Assert.Contains("Invalid identifier", ex.Message);
        }

        [Fact]
        public void GenerateInsertQuery_WithEmptyRowData_ThrowsException()
        {
            var queryBuilder = new MySQLQueryBuilder();

            var ex = Assert.Throws<ArgumentException>(() =>
                queryBuilder.GenerateInsertQuery("some_table", new Dictionary<string, object>()));

            Assert.Equal("No data provided for insert.", ex.Message);
        }

        [Fact]
        public void GenerateInsertQuery_WithNullTableName_ThrowsException()
        {
            var queryBuilder = new MySQLQueryBuilder();
            var data = new Dictionary<string, object> { { "username", "king" } };

            var ex = Assert.Throws<ArgumentException>(() =>
                queryBuilder.GenerateInsertQuery(null, data));

            Assert.Equal("Target table is required.", ex.Message);
        }
    }
}
