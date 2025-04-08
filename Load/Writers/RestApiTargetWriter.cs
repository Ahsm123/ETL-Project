using ETL.Domain.Targets.ApiTargets;
using ETL.Domain.Targets;
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

    public async Task WriteAsync(TargetInfoBase targetInfo, Dictionary<string, object> data)
    {
        try
        {
            if (targetInfo is not RestApiTargetInfo apiInfo)
                throw new ArgumentException("Invalid target info type");

            _logger.LogInformation("[API] Sending {Method} to {Url}", apiInfo.Method, apiInfo.Url);

            // Her ville der være et HTTP-kald
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write to REST API target.");
            throw;
        }
    }
}
