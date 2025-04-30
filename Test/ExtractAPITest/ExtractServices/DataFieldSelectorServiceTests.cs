using ExtractAPI.Services;
using System.Text.Json;

namespace Test.ExstractAPITest.ExtractServices;

public class DataFieldSelectorServiceTests
{
    private readonly DataFieldSelectorService _service = new();

    [Fact]
    public void FilterFields_WhenFieldsAreSpecified_ReturnsOnlyMatchingFields()
    {
        // Arrange
        var json = """
        [
            { "account_id": 1, "cost": 100, "extra": "ignored" },
            { "account_id": 2, "cost": 200, "extra": "ignored" }
        ]
        """;

        using var doc = JsonDocument.Parse(json);
        var data = doc.RootElement;

        var fields = new List<string> { "account_id", "cost" };

        // Act
        var result = _service.SelectRecords(data, fields).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(2, result[0].Fields.Count);
        Assert.Contains("account_id", result[0].Fields.Keys);
        Assert.Contains("cost", result[0].Fields.Keys);
        Assert.DoesNotContain("extra", result[0].Fields.Keys);
    }

    [Fact]
    public void FilterFields_WithEmptyFieldList_ReturnsEmptyRecords()
    {
        // Arrange
        var json = "[{ \"id\": 1, \"name\": \"test\" }]";
        using var doc = JsonDocument.Parse(json);
        var data = doc.RootElement;

        // Act
        var result = _service.SelectRecords(data, new List<string>()).ToList();

        // Assert
        Assert.Empty(result); 
    }



    [Fact]
    public void FilterFields_WithNonExistentFields_ReturnsEmptyRecords()
    {
        // Arrange
        var json = "[{ \"name\": \"hello\" }]";
        using var doc = JsonDocument.Parse(json);
        var data = doc.RootElement;
        var fields = new List<string> { "missing_field" };

        // Act
        var result = _service.SelectRecords(data, fields).ToList();

        // Assert
        Assert.Single(result);
        Assert.Empty(result[0].Fields);
    }

}
