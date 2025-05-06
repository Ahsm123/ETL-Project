using Dapper;
using ETL.Domain.Rules;
using ETL.Domain.Sources;
using ETL.Domain.SQLQueryBuilder;
using ExtractAPI.DataSources.DatabaseQueryBuilder.Interfaces;

namespace Test.ETLConfig.APITest;

public class MsSqlQueryBuilderTests
{
    private readonly ISqlQueryBuilder _queryBuilder;

    public MsSqlQueryBuilderTests()
    {
        _queryBuilder = new MsSqlQueryBuilder();
    }

    [Fact]
    public void GenerateSelectQuery_WithValidFieldsAndFilters_ReturnsCorrectSql()
    {
        // Arrange
        var source = new MsSqlSourceInfo
        {
            TargetTable = "Users"
        };

        var fields = new List<string> { "Id", "Name" };
        var filters = new List<FilterRule>
        {
            new("Role", "equals", "Admin")
        };

        // Act
        var (query, parameters) = _queryBuilder.GenerateSelectQuery(source, fields, filters);

        // Assert
        Assert.Equal("SELECT [Id], [Name] FROM [Users] WHERE [Role] = @param0", query);
        Assert.IsType<DynamicParameters>(parameters);
        Assert.Equal("Admin", parameters.Get<string>("@param0"));
    }

    [Fact]
    public void GenerateSelectQuery_WithNoFields_ReturnsSelectAll()
    {
        // Arrange
        var source = new MsSqlSourceInfo
        {
            TargetTable = "Users"
        };

        // Act
        var (query, _) = _queryBuilder.GenerateSelectQuery(source, null, null);

        // Assert
        Assert.Equal("SELECT * FROM [Users]", query);
    }

    [Fact]
    public void GenerateSelectQuery_WithInvalidFieldName_ThrowsException()
    {
        // Arrange
        var source = new MsSqlSourceInfo
        {
            TargetTable = "Users"
        };

        var invalidFields = new List<string> { "Id", "DROP TABLE Users" };

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            _queryBuilder.GenerateSelectQuery(source, invalidFields, null));
    }

    [Fact]
    public void GenerateSelectQuery_WithUnsupportedFilterOperator_ThrowsException()
    {
        // Arrange
        var source = new MsSqlSourceInfo
        {
            TargetTable = "Orders"
        };

        var filters = new List<FilterRule>
        {
            new("Price", "unsupported_op", "100")
        };

        // Act & Assert
        Assert.Throws<NotSupportedException>(() =>
            _queryBuilder.GenerateSelectQuery(source, new List<string> { "Id" }, filters));
    }
}