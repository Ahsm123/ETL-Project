using ETL.Domain.Events;
using ETL.Domain.Json;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Transform.Interfaces;

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

    public async Task<string> TransformDataAsync(ExtractedEvent input)
    {
        try
        {
            var processed = _pipeline.Run(input);

            if (processed is null)
            {
                _logger.LogWarning("Payload {Id} was filtered out by the pipeline", input.Id);
                return "{}";
            }

            var result = new TransformedEvent
            {
                PipelineId = processed.PipelineId,
                LoadTargetConfig = processed.LoadTargetConfig,
                Data = processed.Data
            };

            return JsonSerializer.Serialize(result, JsonOptionsFactory.Default);
        }
        catch (JsonException jsonEx)
        {
            _logger.LogError(jsonEx, "JSON serialization failed for event with ID: {Id}", input.Id);
            return "{}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while transforming data for event with ID: {Id}", input.Id);
            return "{}";
        }
    }
}
