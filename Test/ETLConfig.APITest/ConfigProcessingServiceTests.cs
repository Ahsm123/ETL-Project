using ETL.Domain.JsonHelpers;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Test.ETLConfig.APITest;
public class ConfigProcessingServiceTests
{
    private readonly ConfigProcessingService _service;
    private readonly FakeConfigRepository _repo = new();
    private readonly IJsonService _jsonService = new JsonService();

    public ConfigProcessingServiceTests()
    {
        _service = new ConfigProcessingService(_repo, _jsonService);
    }


    [Fact]
    public async Task ProcessSingleConfigAsync_MissingId_ThrowsValidation()
    {
        var json = """
        {
            "ExtractConfig": {
                "SourceInfo": { "$type": "restapi", "Url": "https://example.com" },
                "Fields": ["x"]
            },
            "TransformConfig": {},
            "LoadTargetConfig": {
                "TargetInfo": { "$type": "mssql", "ConnectionString": "conn", "TargetTable": "Customers" }
            }
        }
        """;

        var parsed = JsonDocument.Parse(json).RootElement;

        await Assert.ThrowsAsync<ValidationException>(() =>
            _service.ProcessSingleConfigAsync(parsed));
    }

    [Fact]
    public async Task ProcessSingleConfigAsync_MissingSourceInfo_ThrowsValidation()
    {
        var json = """
        {
            "Id": "no_sourceinfo",
            "ExtractConfig": {
                "Fields": ["field"]
            },
            "TransformConfig": {},
            "LoadTargetConfig": {
                "TargetInfo": { "$type": "mssql", "ConnectionString": "conn", "TargetTable": "Customers" }
            }
        }
        """;

        var parsed = JsonDocument.Parse(json).RootElement;

        await Assert.ThrowsAsync<ValidationException>(() =>
            _service.ProcessSingleConfigAsync(parsed));
    }

    [Fact]
    public async Task ProcessSingleConfigAsync_MissingTargetInfo_ThrowsValidation()
    {
        var json = """
        {
            "Id": "no_targetinfo",
            "ExtractConfig": {
                "SourceInfo": { "$type": "restapi", "Url": "https://example.com" },
                "Fields": ["x"]
            },
            "TransformConfig": {},
            "LoadTargetConfig": {}
        }
        """;

        var parsed = JsonDocument.Parse(json).RootElement;

        await Assert.ThrowsAsync<ValidationException>(() =>
            _service.ProcessSingleConfigAsync(parsed));
    }

    [Fact]
    public async Task ProcessSingleConfigAsync_MissingRequiredFields_ThrowsValidationException()
    {
        var json = "{}";
        var root = JsonDocument.Parse(json).RootElement;

        await Assert.ThrowsAsync<ValidationException>(() =>
            _service.ProcessSingleConfigAsync(root));
    }




}
