using ETL.Domain.Rules;
using ETL.Domain.Sources;
using ETL.Domain.SQLQueryBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.ETLConfig.API;
public class MsSqlQueryBuilderTests
{
    [Fact]
    public void BuildQuery_WithFieldsAndFilters_BuildsCorrectSql()
    {
        var source = new MsSqlSourceInfo
        {
            TargetTable = "Users"
        };

        var fields = new List<string> { "Id", "Name" };
        var filters = new List<FilterRule>
        {
            new() { Field = "Role", Operator = "equals", Value = "Admin" }
        };

        var (query, parameters) = MsSqlQueryBuilder.BuildSafeSelectQuery(source, fields, filters);

        Assert.Equal("SELECT [Id], [Name] FROM [Users] WHERE [Role] = @param0", query);
        Assert.Single(parameters);
        Assert.Equal("Admin", parameters["@param0"]);
    }

    [Fact]
    public void BuildQuery_WithoutFields_ReturnsSelectAll()
    {
        var source = new MsSqlSourceInfo { TargetTable = "Users" };
        var (query, _) = MsSqlQueryBuilder.BuildSafeSelectQuery(source, null, null);

        Assert.Equal("SELECT * FROM [Users]", query);
    }

    [Fact]
    public void BuildQuery_WithInvalidField_ThrowsException()
    {
        var source = new MsSqlSourceInfo { TargetTable = "Users" };

        var fields = new List<string> { "Id", "Name;", "DROP TABLE" };

        Assert.Throws<ArgumentException>(() =>
            MsSqlQueryBuilder.BuildSafeSelectQuery(source, fields, new()));
    }

    [Fact]
    public void BuildQuery_WithUnsupportedOperator_Throws()
    {
        var source = new MsSqlSourceInfo { TargetTable = "Products" };
        var filters = new List<FilterRule>
        {
            new() { Field = "Price", Operator = "between", Value = "10 AND 20" }
        };

        Assert.Throws<NotSupportedException>(() =>
            MsSqlQueryBuilder.BuildSafeSelectQuery(source, new List<string>(), filters));
    }
}

