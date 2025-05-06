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
            new FieldMapRule{ SourceField = "account_id", TargetField = "id"}
        };

        // Act 
        var result = _mappingService.Apply(input, mappings);

        // Assert 
        Assert.False(result.Fields.ContainsKey("account_id"));
        Assert.True(result.Fields.ContainsKey("id"));
        Assert.Equal(123, result.Fields["id"]);
        Assert.Equal("Accepted", result.Fields["status"]);
    }

    [Fact]
    public void Apply_WhenMultipleMappingsProvided_MapsAllFieldsCorrectly()
    {
        var input = new RawRecord(new Dictionary<string, object>
    {
        { "account_id", 123 },
        { "customer_status", "active" },
        { "region", "EU" }
    });

        var mappings = new List<FieldMapRule>
    {
             new FieldMapRule{SourceField = "account_id", TargetField = "id"},
              new FieldMapRule{SourceField = "customer_status", TargetField = "status"}
    };

        var service = new MappingService();
        var result = service.Apply(input, mappings);

        Assert.False(result.Fields.ContainsKey("account_id"));
        Assert.False(result.Fields.ContainsKey("customer_status"));
        Assert.Equal(123, result.Fields["id"]);
        Assert.Equal("active", result.Fields["status"]);
        Assert.Equal("EU", result.Fields["region"]);
    }

    [Fact]
    public void Apply_WhenSourceFieldIsMissing_DoesNotThrowAndSkips()
    {
        var input = new RawRecord(new Dictionary<string, object>
    {
        { "email", "test@example.com" }
    });

        var mappings = new List<FieldMapRule>
    {
        new FieldMapRule{SourceField = "account_id", TargetField = "id"}

    };

        var service = new MappingService();
        var result = service.Apply(input, mappings);

        // Should not throw or crash
        Assert.False(result.Fields.ContainsKey("id")); // no "account_id" to map
        Assert.Equal("test@example.com", result.Fields["email"]);
    }

    [Fact]
    public void Apply_WhenTargetFieldAlreadyExists_OverwritesIt()
    {
        var input = new RawRecord(new Dictionary<string, object>
    {
        { "account_id", 123 },
        { "id", 999 } // already exists
    });

        var mappings = new List<FieldMapRule>
    {
         new FieldMapRule{SourceField = "account_id", TargetField = "id"}
    };

        var service = new MappingService();
        var result = service.Apply(input, mappings);

        Assert.Equal(123, result.Fields["id"]); // overwritten by mapping
    }



}
