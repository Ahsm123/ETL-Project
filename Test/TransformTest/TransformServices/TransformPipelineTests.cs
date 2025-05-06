using ETL.Domain.Config;
using ETL.Domain.Events;
using ETL.Domain.Model;
using ETL.Domain.Rules;
using Microsoft.Extensions.Logging;
using Moq;
using Transform.Interfaces;
using Transform.Services;

namespace Test.TransformTest.TransformServices;

public class TransformPipelineTests
{
    private readonly Mock<ILogger<TransformPipeline>> _loggerMock = new();
    private readonly FilterService _filterService = new();
    private readonly MappingService _mappingService = new();
    private readonly TransformPipeline _pipeline;

    public TransformPipelineTests()
    {
        _pipeline = new TransformPipeline(_mappingService, _filterService, _loggerMock.Object);
    }

    [Fact]
    public void Run_WhenRecordFailsFilter_ReturnsNull()
    {
        // Arrange
        var input = new ExtractedEvent
        {
            PipelineId = Guid.NewGuid().ToString(),
            Record = new RawRecord(new Dictionary<string, object>
            {
                { "value", 50 }
            }),
            TransformConfig = new TransformConfig
            {
                Filters = new List<FilterRule>
                {
                    new("value", "greaterthan", "100") // fails
                },
                Mappings = new()
            },
            LoadTargetConfig = new LoadConfig()
        };

        // Act
        var result = _pipeline.Run(input);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Run_WhenRecordPassesFilterAndIsMapped_ReturnsTransformedEvent()
    {
        // Arrange
        var input = new ExtractedEvent
        {
            PipelineId = Guid.NewGuid().ToString(),
            Record = new RawRecord(new Dictionary<string, object>
            {
                { "account_id", 123 },
                { "status", "Accepted" }
            }),
            TransformConfig = new TransformConfig
            {
                Filters = new List<FilterRule>
                {
                    new("status", "equals", "Accepted")
                },
                Mappings = new List<FieldMapRule>
                {
                    new FieldMapRule{SourceField = "account_id", TargetField = "id"}
                }
            },
            LoadTargetConfig = new LoadConfig()
        };

        // Act
        var result = _pipeline.Run(input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(input.PipelineId, result.PipelineId);
        Assert.Equal(input.LoadTargetConfig, result.LoadTargetConfig);
        Assert.True(result.Record.Fields.ContainsKey("id"));
        Assert.False(result.Record.Fields.ContainsKey("account_id"));
        Assert.Equal(123, result.Record.Fields["id"]);
    }
}

