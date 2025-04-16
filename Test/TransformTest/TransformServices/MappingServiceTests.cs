using ETL.Domain.Events;
using ETL.Domain.Rules;
using Transform.Services;

namespace Test.TransformTest.TransformServices;

public class MappingServiceTests
{
    private readonly MappingService _mappingService = new();

    [Fact]
    public void Apply_MapsFieldAndRemovesOriginal()
    {
        // Arrange 
        var input = new RawRecord(new Dictionary<string, object>
        {
            { "account_id", 123 },
            { "status", "Accepted" }
        });

        var mappings = new List<FieldMapRule>
        {
            new("account_id", "id")
        };

        // Act 
        var result = _mappingService.Apply(input, mappings);

        // Assert 
        Assert.False(result.Fields.ContainsKey("account_id"));
        Assert.True(result.Fields.ContainsKey("id"));
        Assert.Equal(123, result.Fields["id"]);
        Assert.Equal("Accepted", result.Fields["status"]);
    }
}
