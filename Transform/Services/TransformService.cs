using ETL.Domain.Events;
using Microsoft.Extensions.Logging;
using System.Text.Json;

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
            SourceType = processed.SourceType,
            LoadTargetConfig = processed.LoadTargetConfig,
            Data = processed.Data
        };


        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        return Task.FromResult(JsonSerializer.Serialize(resultToSerialize, options));
    }
}





