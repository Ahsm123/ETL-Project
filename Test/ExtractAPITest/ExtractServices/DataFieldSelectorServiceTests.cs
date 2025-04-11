using ETL.Domain.Rules;
using ExtractAPI.Services;
using System.Text.Json;
using Transform.Services;

namespace Test.ExstractAPITest.ExtractServices;

public class DataFieldSelectorServiceTests
{
    private readonly DataFieldSelectorService _service = new();

    [Fact]
    public void FilterFields_ReturnsOnlySpecifiedFields()
    {
        // Arrange
        var json = """
            [
                { "account_id": 1, "cost": 100, "extra": "ignored" },
                { "account_id": 2, "cost": 200, "extra": "ignored" }
            ]
            """;

        var data = JsonSerializer.Deserialize<JsonElement>(json);
        var fields = new List<string> { "account_id", "cost" };

        // Act
        var result = _service.FilterFields(data!, fields).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(2, result[0].Count);
        Assert.Contains("account_id", result[0]);
        Assert.Contains("cost", result[0]);
        Assert.DoesNotContain("extra", result[0]);
    }

    [Fact]
    public void FilterFields_EmptyFields_ReturnsNoResults()
    {
        var json = "[{ \"id\": 1, \"name\": \"test\" }]";
        var data = JsonSerializer.Deserialize<JsonElement>(json);

        var result = _service.FilterFields(data!, new List<string>()).ToList();

        Assert.Empty(result);
    }


    [Fact]
    public void FilterFields_NonExistentFields_ReturnsEmptyObjects()
    {
        //Arrange
        var json = "[{ \"name\": \"hello\" }]";
        var data = JsonSerializer.Deserialize<JsonElement>(json);
        var fields = new List<string> { "missing_field" };

        //Act
        var result = _service.FilterFields(data!, fields).ToList();

        //Assert
        Assert.Single(result);
        Assert.Empty(result[0]);
    }
    [Fact]
    public void ShouldInclude_ReturnsFalse_WhenFieldIsMissing()
    {
        // Arrange
        var filterService = new FilterService();
        var item = new Dictionary<string, object>(); // Tomt item

        var filters = new List<FilterRule>
    {
        new FilterRule { Field = "nonexistent", Operator = "equals", Value = "test" }
    };

        // Act
        var result = filterService.ShouldInclude(item, filters);

        // Assert
        Assert.False(result);
    }
    [Fact]
    public void ShouldInclude_ReturnsFalse_WhenOperatorIsUnknown()
    {
        // Arrange
        var filterService = new FilterService();
        var item = new Dictionary<string, object>
    {
        { "status", JsonDocument.Parse("\"Accepted\"").RootElement }
    };

        var filters = new List<FilterRule>
    {
        new FilterRule { Field = "status", Operator = "notanoperator", Value = "Accepted" }
    };

        // Act
        var result = filterService.ShouldInclude(item, filters);

        // Assert
        Assert.False(result);
    }
    [Fact]
    public void ShouldInclude_ReturnsFalse_WhenFieldValueIsNull()
    {
        // Arrange
        var filterService = new FilterService();
        var item = new Dictionary<string, object>
    {
        { "status", JsonDocument.Parse("null").RootElement }
    };

        var filters = new List<FilterRule>
    {
        new FilterRule { Field = "status", Operator = "equals", Value = "Accepted" }
    };

        // Act
        var result = filterService.ShouldInclude(item, filters);

        // Assert
        Assert.False(result);
    }
    [Fact]
    public void ShouldInclude_ReturnsFalse_IfOneOfMultipleFiltersFails()
    {
        // Arrange
        var filterService = new FilterService();
        var item = new Dictionary<string, object>
    {
        { "cost", JsonDocument.Parse("1500").RootElement },
        { "status", JsonDocument.Parse("\"Declined\"").RootElement }
    };

        var filters = new List<FilterRule>
    {
        new FilterRule { Field = "cost", Operator = "greaterthan", Value = "1000" },
        new FilterRule { Field = "status", Operator = "equals", Value = "Accepted" }
    };

        // Act
        var result = filterService.ShouldInclude(item, filters);

        // Assert
        Assert.False(result);
    }
    [Theory]
    [InlineData("Declined", true)]
    [InlineData("Accepted", false)]
    public void ShouldInclude_Notequals_WorksAsExpected(string status, bool expected)
    {
        var filterService = new FilterService();
        var item = new Dictionary<string, object>
    {
        { "status", JsonDocument.Parse($"\"{status}\"").RootElement }
    };

        var filters = new List<FilterRule>
    {
        new FilterRule { Field = "status", Operator = "notequals", Value = "Accepted" }
    };

        var result = filterService.ShouldInclude(item, filters);
        Assert.Equal(expected, result);
    }

}
