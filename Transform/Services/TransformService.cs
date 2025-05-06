using ETL.Domain.Events;
using ETL.Domain.JsonHelpers;
using Microsoft.Extensions.Logging;
using Transform.Services.Interfaces;

namespace Transform.Services;

public class TransformService : ITransformService<string>
{
    private readonly ITransformPipeline _pipeline;
    private readonly ILogger<TransformService> _logger;
    private readonly IJsonService _jsonService;

    public TransformService(
     ITransformPipeline pipeline,
     IJsonService jsonService,
     ILogger<TransformService> logger)
    {
        _pipeline = pipeline;
        _jsonService = jsonService;
        _logger = logger;
    }

    public Task<string> TransformDataAsync(ExtractedEvent input)
    {
        try
        {
            var processed = _pipeline.Run(input);

            if (processed is null)
            {
                _logger.LogWarning("Payload {Id} was filtered out by the pipeline", input.PipelineId);
                return Task.FromResult("{}");
            }

            var json = _jsonService.Serialize(processed);
            return Task.FromResult(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while transforming data for event with ID: {Id}", input.PipelineId);
            return Task.FromResult("{}");
        }
    }

}
