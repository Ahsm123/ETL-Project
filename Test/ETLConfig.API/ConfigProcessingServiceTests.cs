using ETLConfig.API.Services;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Test.ETLConfig.API;
public class ConfigProcessingServiceTests
{
    private readonly ConfigProcessingService _service;

    public ConfigProcessingServiceTests()
    {
        _service = new ConfigProcessingService(new FakeConfigRepository());
    }

    [Fact]
    public async Task ValidateConfig_WithValidConfig_DoesNotThrow()
    {
        // Arrange
        var jsonConfig = """
        {
            "Id": "pipeline_valid",
            "ExtractConfig": {
                "SourceInfo": { "$type": "restapi", "Url": "https://api.example.com" },
                "Fields": ["name", "age"]
            },
            "TransformConfig": {},
            "LoadTargetConfig": {
                "TargetInfo": { "$type": "mssql", "ConnectionString": "Server=myDb;...", "TargetTable": "People" }
            }
        } 
        """;

        var parsedConfig = JsonDocument.Parse(jsonConfig).RootElement;

        // Act
        var result = await _service.ProcessSingleConfigAsync(parsedConfig);

        // Assert
        Assert.Equal("pipeline_valid", result.Id);

    }

    [Fact]
    public async Task ValidateConfig_MissingId_ThrowsValidationException()
    {
        // Arrange
        var jsonConfig = """
        {
            "ExtractConfig": {
                "SourceInfo": { "$type": "restapi", "Url": "https://api.example.com" },
                "Fields": ["name"]
            },
            "TransformConfig": {},
            "LoadTargetConfig": {
                "TargetInfo": { "$type": "mssql", "ConnectionString": "conn", "TargetTable": "Customers" }
            }
        }
        """;

        var parsedConfig = JsonDocument.Parse(jsonConfig).RootElement;

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() =>
            _service.ProcessSingleConfigAsync(parsedConfig));
    }

    [Fact]
    public async Task ValidateConfig_MissingExtractConfig_ThrowsValidationException()
    {
        var jsonConfig = """
        {
            "Id": "pipeline_missing_extract",
            "TransformConfig": {},
            "LoadTargetConfig": {
                "TargetInfo": { "$type": "mssql", "ConnectionString": "conn", "TargetTable": "table" }
            }
        }
        """;

        var parsedConfig = JsonDocument.Parse(jsonConfig).RootElement;

        await Assert.ThrowsAsync<ValidationException>(() =>
            _service.ProcessSingleConfigAsync(parsedConfig));
    }

    [Fact]
    public async Task ValidateConfig_MissingSourceInfo_ThrowsValidationException()
    {
        var jsonConfig = """
    {
        "Id": "pipeline_no_sourceinfo",
        "ExtractConfig": {
            "Fields": ["name"]
        },
        "TransformConfig": {},
        "LoadTargetConfig": {
            "TargetInfo": { "$type": "mssql", "ConnectionString": "conn", "TargetTable": "Customers" }
        }
    }
    """;

        var parsedConfig = JsonDocument.Parse(jsonConfig).RootElement;

        await Assert.ThrowsAsync<ValidationException>(() =>
            _service.ProcessSingleConfigAsync(parsedConfig));
    }

}
