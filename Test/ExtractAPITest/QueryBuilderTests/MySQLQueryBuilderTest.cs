using Dapper;
using DocumentFormat.OpenXml.Drawing;
using ETL.Domain.Rules;
using ETL.Domain.Sources;
using ETL.Domain.Targets.DbTargets;
using ExtractAPI.DataSources.DatabaseQueryBuilder;
using System.Runtime.InteropServices;
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
                Fields = new List<FieldMapRule>
                {
                    new FieldMapRule { SourceField = "account_id", TargetField = "account_id" },
                    new FieldMapRule { SourceField = "cost", TargetField = "cost" },
                    new FieldMapRule { SourceField = "status", TargetField = "status" }
                }
            };
        }

        [Fact]
        public void GenerateSelectQuery_GeneratesValidSqlWithoutFilter()
        {
            var queryBuilder = new MySQLQueryBuilder();
            var table = GetStandardTable();

            var sourceInfo = new MySQLSourceInfo
            {
                TargetTable = table.TargetTable,
            };
            var rowData = new List<string> { "account_id", "cost", "status" };

            var (sql, parameters) = queryBuilder.GenerateSelectQuery(sourceInfo, rowData, null);
            var expectedSql = "SELECT `account_id`, `cost`, `status` FROM `approved_highvalue_payments`;";
            Assert.Equal(expectedSql, sql.Trim());
        }

        [Fact]
        public void GenerateSelectQuery_GeneratesValidSqlAndParameterWithFilters()
        {
            var queryBuilder = new MySQLQueryBuilder();
            var table = GetStandardTable();

            var sourceInfo = new MySQLSourceInfo
            {
                TargetTable = table.TargetTable,
            };
            var rowData = new List<string> { "account_id", "cost", "status" };
            var filterRules = new List<FilterRule>
            {
                new FilterRule("cost","less_than","1000"),
                new FilterRule("status","equals","approved")
            };

            var (sql, parameters) = queryBuilder.GenerateSelectQuery(sourceInfo, rowData,filterRules);
            var expectedSql = "SELECT `account_id`, `cost`, `status` FROM `approved_highvalue_payments` WHERE `cost` < @p0 AND `status` = @p1;";
            Assert.Equal(expectedSql, sql.Trim());

        }

        [Fact]
        public void GenerateSelectQuery_GeneratesValidSqlWithEmptyFields()
        {
            var queryBuilder = new MySQLQueryBuilder();
            var table = GetStandardTable();

            var sourceInfo = new MySQLSourceInfo
            {
                TargetTable = table.TargetTable,
            };
            var rowData = new List<string>();

            var (sql, parameters) = queryBuilder.GenerateSelectQuery(sourceInfo, rowData, null);
            var expectedSql = "SELECT * FROM `approved_highvalue_payments`;";
            Assert.Equal(expectedSql, sql.Trim());
        }
        [Fact]
        public void GenerateSelectQuery_ThrowsException_WhenEmptyTargetTable()
        {
            var queryBuilder = new MySQLQueryBuilder();
            var sourceInfo = new MySQLSourceInfo
            {
                TargetTable = null,
            };
            var rowData = new List<string> { "account_id", "cost", "status" };

            var ex = Assert.Throws<ArgumentException>(() =>
                queryBuilder.GenerateSelectQuery(sourceInfo, rowData, null));

            Assert.Equal("Target table is required.", ex.Message);
        }

        [Fact]
        public void GenerateSelectQuery_WhenFilterRuleIsNull_ThrowsException()
        {
            var queryBuilder = new MySQLQueryBuilder();
            var table = GetStandardTable();
            var sourceInfo = new MySQLSourceInfo
            {
                TargetTable = table.TargetTable
            };
            var rowData = new List<string> { "account_id", "cost", "status" };
            var filterRules = new List<FilterRule>
            {
                new FilterRule("cost", "null", "1000"),
                
            };
            var ex = Assert.Throws<ArgumentException>(() =>
            queryBuilder.GenerateSelectQuery(sourceInfo, rowData, filterRules));

            Assert.Contains("Unsupported operator", ex.Message);
        }



            [Fact]
        public void GenerateInsertQuery_ReturnsCorrectSqlAndParametersWhenValidInput()
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
        public void GenerateInsertQuery_TableNameWithSqlInjection_ThrowsException()
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
        public void GenerateInsertQuery_NullTableName_ThrowsException()
        {
            var queryBuilder = new MySQLQueryBuilder();
            var data = new Dictionary<string, object> { { "username", "king" } };

            var ex = Assert.Throws<ArgumentException>(() =>
                queryBuilder.GenerateInsertQuery(null, data));

            Assert.Equal("Target table is required.", ex.Message);
        }
    }
}
