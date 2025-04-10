using Dapper;
using ETL.Domain.Rules;
using ETL.Domain.Sources;
using ETL.Domain.SQLQueryBuilder;
using ExtractAPI.DataSources.DatabaseQueryBuilder.Interfaces;

namespace Test.ETLConfig.API;

public class MsSqlQueryBuilderTests
{
    private readonly ISqlQueryBuilder _queryBuilder;

    public MsSqlQueryBuilderTests()
    {
        _queryBuilder = new MsSqlQueryBuilder();
    }

    [Fact]
    public void BuildSelectQuery_WithFieldsAndFilters_BuildsCorrectSql()
    {
        // Arrange
        var source = new MsSqlSourceInfo
        {
            TargetTable = "Users"
        };

        var fields = new List<string> { "Id", "Name" };
        var filters = new List<FilterRule>
    {
        new() { Field = "Role", Operator = "equals", Value = "Admin" }
    };

        // Act
        var (query, parameters) = _queryBuilder.GenerateSelectQuery(source, fields, filters);

        // Assert
        Assert.Equal("SELECT [Id], [Name] FROM [Users] WHERE [Role] = @param0", query);
        Assert.IsType<DynamicParameters>(parameters);

        var paramValue = parameters.Get<object>("@param0");
        Assert.Equal("Admin", paramValue);
    }


    [Fact]
    public void BuildSelectQuery_WithoutFields_ReturnsSelectAll()
    {
        var source = new MsSqlSourceInfo { TargetTable = "Users" };

        var (query, _) = _queryBuilder.GenerateSelectQuery(source, null, null);

        Assert.Equal("SELECT * FROM [Users]", query);
    }

    [Fact]
    public void BuildSelectQuery_WithInvalidField_ThrowsException()
    {
        var source = new MsSqlSourceInfo { TargetTable = "Users" };

        var fields = new List<string> { "Id", "Name;", "DROP TABLE" };

        Assert.Throws<ArgumentException>(() =>
            _queryBuilder.GenerateSelectQuery(source, fields, new()));
    }

    [Fact]
    public void BuildSelectQuery_WithUnsupportedOperator_ThrowsException()
    {
        var source = new MsSqlSourceInfo { TargetTable = "Products" };
        var filters = new List<FilterRule>
        {
            new() { Field = "Price", Operator = "between", Value = "10 AND 20" }
        };

        Assert.Throws<NotSupportedException>(() =>
            _queryBuilder.GenerateSelectQuery(source, new List<string>(), filters));
    }
}
