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
        try
        {
            var result = _pipeline.Execute(input);
            return Task.FromResult(JsonSerializer.Serialize(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fejl under transformation af payload");
            throw;
        }
    }
}





