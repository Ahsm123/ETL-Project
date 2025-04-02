using ETL.Domain.Rules;
using Transform.Services;

namespace Test.Tests.TransformServices;

public class MappingServiceTests
{
    private readonly MappingService _mappingService = new();

    [Fact]
    public void Apply_MapsFieldAndRemovesOriginal()
    {
        // Arrange 
        var item = new Dictionary<string, object> { { "account_id", 123 }, { "status", "Accepted" } };
        var mappings = new List<FieldMapRule>
            {
                new() { SourceField = "account_id", TargetField = "id" }
            };

        // Act 
        var result = _mappingService.Apply(item, mappings);

        // Assert 
        Assert.False(result.ContainsKey("account_id"));
        Assert.True(result.ContainsKey("id"));
        Assert.Equal(123, result["id"]);
        Assert.Equal("Accepted", result["status"]);
    }
}