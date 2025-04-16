using ETL.Domain.Targets;
using ETL.Domain.Targets.ApiTargets;
using Load.Interfaces;
using Microsoft.Extensions.Logging;

namespace Load.Writers;

public class RestApiTargetWriter : ITargetWriter
{
    private readonly ILogger<RestApiTargetWriter> _logger;

    public RestApiTargetWriter(ILogger<RestApiTargetWriter> logger)
    {
        _logger = logger;
    }

    public bool CanHandle(Type targetInfoType)
        => typeof(RestApiTargetInfo).IsAssignableFrom(targetInfoType);

    public async Task WriteAsync(TargetInfoBase targetInfo, Dictionary<string, object> data, string? pipelineId = null)
    {
        try
        {
            if (targetInfo is not RestApiTargetInfo apiInfo)
                throw new ArgumentException("Invalid target info type");

            _logger.LogInformation("[API] Would send {Method} to {Url} with data: {Data}",
                apiInfo.Method, apiInfo.Url, data);

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write to REST API target.");
            throw;
        }
    }
}
