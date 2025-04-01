using ExtractAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Test.Tests.Services;

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
    public void FilterFields_EmptyFields_ReturnsEmptyObjects()
    {
        //Arrange
        var json = "[{ \"id\": 1, \"name\": \"test\" }]";
        var data = JsonSerializer.Deserialize<JsonElement>(json);

        //Act
        var result = _service.FilterFields(data!, new List<string>()).ToList();

        //Assert
        Assert.Single(result);
        Assert.Empty(result[0]);
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
}
