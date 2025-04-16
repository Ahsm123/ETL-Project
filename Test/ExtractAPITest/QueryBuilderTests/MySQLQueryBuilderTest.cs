using Dapper;
using ETL.Domain.Rules;
using ETL.Domain.Sources;
using ETL.Domain.Targets.DbTargets;
using ExtractAPI.DataSources.DatabaseQueryBuilder;

namespace Test.ExtractAPITest.QueryBuilderTests;

public class MySQLQueryBuilderTest
{
    [Fact]
    public void GenerateSelectQuery_GeneratesValidSqlAndParameters()
    {
        var queryBuilder = new MySQLQueryBuilder();
        var sourceInfo = new MySQLSourceInfo { TargetTable = "approved_highvalue_payments" };
        var fields = new List<string> { "account_id", "cost", "status" };
        var filters = new List<FilterRule>
        {
            new("cost", "greater_than", "1000"),
            new("status", "equals", "approved")
        };

        var (sql, parameters) = queryBuilder.GenerateSelectQuery(sourceInfo, fields, filters);

        var expectedSql = "SELECT `account_id`, `cost`, `status` FROM `approved_highvalue_payments` WHERE `cost` > @p0 AND `status` = @p1;";
        Assert.Equal(expectedSql, sql.Trim());
        Assert.Equal("1000", parameters.Get<string>("@p0"));
        Assert.Equal("approved", parameters.Get<string>("@p1"));
    }

    [Fact]
    public void GenerateSelectQuery_GeneratesValidSQLWithoutFilter()
    {
        var queryBuilder = new MySQLQueryBuilder();
        var sourceInfo = new MySQLSourceInfo { TargetTable = "approved_highvalue_payments" };
        var fields = new List<string> { "account_id", "cost", "status" };

        var (sql, _) = queryBuilder.GenerateSelectQuery(sourceInfo, fields, null);

        Assert.Equal("SELECT `account_id`, `cost`, `status` FROM `approved_highvalue_payments`;", sql.Trim());
    }

    [Fact]
    public void GenerateSelectQuery_GeneratesValidSQLWithEmptyFields()
    {
        var queryBuilder = new MySQLQueryBuilder();
        var sourceInfo = new MySQLSourceInfo { TargetTable = "approved_highvalue_payments" };
        var fields = new List<string>();

        var (sql, _) = queryBuilder.GenerateSelectQuery(sourceInfo, fields, null);

        Assert.Equal("SELECT * FROM `approved_highvalue_payments`;", sql.Trim());
    }

    [Fact]
    public void GenerateSelectQuery_ThrowsExceptionWhenEmptyEmptyTargetTable()
    {
        var queryBuilder = new MySQLQueryBuilder();
        var sourceInfo = new MySQLSourceInfo { TargetTable = "" };
        var fields = new List<string> { "account_id", "cost", "status" };

        var ex = Assert.Throws<ArgumentException>(() =>
            queryBuilder.GenerateSelectQuery(sourceInfo, fields, null));

        Assert.Equal("Target table is required", ex.Message);
    }

    [Fact]
    public void GenerateSelectQuery_WhenFilterRuleIsMissingAttribute_ThrowsException()
    {
        var queryBuilder = new MySQLQueryBuilder();
        var sourceInfo = new MySQLSourceInfo { TargetTable = "approved_highvalue_payments" };
        var fields = new List<string> { "account_id", "cost", "status" };

        var filters = new List<FilterRule>
        {
            new("cost", "", "1000") // invalid operator
        };

        var ex = Assert.Throws<ArgumentException>(() =>
            queryBuilder.GenerateSelectQuery(sourceInfo, fields, filters));

        Assert.Equal("Unsupported operator ''", ex.Message);
    }

    [Fact]
    public void GenerateInsertQuery_ReturnsCorrectSqlAndParametersWhenValidInput()
    {
        var queryBuilder = new MySQLQueryBuilder();
        var targetInfo = new MySqlTargetInfo { TargetTable = "approved_highvalue_payments" };
        var rowData = new Dictionary<string, object>
        {
            { "account_id", 123 },
            { "cost", 1500.50 },
            { "status", "approved" }
        };

        var (sql, parameters) = queryBuilder.GenerateInsertQuery(targetInfo, rowData);

        var expectedSql = "INSERT INTO `approved_highvalue_payments` (`account_id`, `cost`, `status`) VALUES (@account_id, @cost, @status);";
        Assert.Equal(expectedSql, sql.Trim());
        Assert.Equal(3, parameters.ParameterNames.AsList().Count);
    }

    [Fact]
    public void GenerateInsertQuery_NullTargetTable_ThrowsArgumentException()
    {
        var queryBuilder = new MySQLQueryBuilder();
        var targetInfo = new MySqlTargetInfo { TargetTable = "" };
        var data = new Dictionary<string, object> { { "username", "king" } };

        var ex = Assert.Throws<ArgumentException>(() =>
            queryBuilder.GenerateInsertQuery(targetInfo, data));

        Assert.Equal("Target table is required", ex.Message);
    }

    [Fact]
    public void GenerateInsertQuery_TableNameWithSqlInjection_ThrowsArgumentException()
    {
        var queryBuilder = new MySQLQueryBuilder();
        var targetInfo = new MySqlTargetInfo { TargetTable = "users; DROP TABLE users;" };
        var data = new Dictionary<string, object> { { "username", "king" } };

        var ex = Assert.Throws<ArgumentException>(() =>
            queryBuilder.GenerateInsertQuery(targetInfo, data));

        Assert.Contains("Invalid identifier", ex.Message);
    }

    [Fact]
    public void GenerateInsertQuery_NullValue_UsesDBNull()
    {
        var queryBuilder = new MySQLQueryBuilder();
        var targetInfo = new MySqlTargetInfo { TargetTable = "logs" };
        var data = new Dictionary<string, object> { { "message", null } };

        var (sql, parameters) = queryBuilder.GenerateInsertQuery(targetInfo, data);

        Assert.Equal("INSERT INTO `logs` (`message`) VALUES (@message);", sql);
        Assert.Null(parameters.Get<object>("@message"));
    }
}
