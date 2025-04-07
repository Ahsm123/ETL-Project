using ETL.Domain.Events;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;
using Transform.Services.Interfaces;

namespace Transform.Services;

public class TransformService : ITransformService<string>
{
    private readonly ITransformPipeline _pipeline;
    private readonly ILogger<TransformService> _logger;

    public TransformService(ITransformPipeline pipeline, ILogger<TransformService> logger)
    {
        _pipeline = pipeline;
        _logger = logger;
    }

    public Task<string> TransformDataAsync(ExtractedEvent input)
    {
        var processed = _pipeline.Execute(input);

        var resultToSerialize = new TransformedEvent
        {
            PipelineId = processed.PipelineId,
            LoadTargetConfig = processed.LoadTargetConfig,
            Data = processed.Data
        };

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        return Task.FromResult(JsonSerializer.Serialize(resultToSerialize, options));
    }
}
