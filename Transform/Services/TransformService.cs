using ETL.Domain.Model.DTOs;
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

    public Task<string> TransformDataAsync(ExtractedPayload input)
    {
        var processed = _pipeline.Execute(input);

        var resultToSerialize = new
        {
            processed.PipelineId,
            processed.SourceType,
            Load = new
            {
                processed.Load.TargetType,
                TargetInfo = (object)processed.Load.TargetInfo
            },
            processed.Data
        };

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        return Task.FromResult(JsonSerializer.Serialize(resultToSerialize, options));
    }
}





