using ETL.Domain.Events;
using ETL.Domain.JsonHelpers;
using Load.Interfaces;
using Microsoft.Extensions.Logging;

namespace Load.Services;

public class LoadHandler : ILoadHandler
{
    private readonly ITargetWriterResolver _targetWriterResolver;
    private readonly IServiceProvider _serviceProvider;
    private readonly IJsonService _jsonService;
    private readonly ILogger<LoadHandler> _logger;

    public LoadHandler(
        ITargetWriterResolver targetWriterResolver,
        IServiceProvider serviceProvider,
        IJsonService jsonService,
        ILogger<LoadHandler> logger)
    {
        _targetWriterResolver = targetWriterResolver;
        _serviceProvider = serviceProvider;
        _jsonService = jsonService;
        _logger = logger;
    }

    public async Task HandleAsync(string json)
    {
        if (string.IsNullOrWhiteSpace(json) || json == "{}")
            throw new InvalidOperationException("Received empty or invalid payload.");

        var payload = _jsonService.Deserialize<TransformedEvent>(json);

        if (payload?.LoadTargetConfig?.TargetInfo == null)
            throw new InvalidOperationException("TargetInfo is missing from payload.");

        var writer = _targetWriterResolver.Resolve(payload.LoadTargetConfig.TargetInfo.GetType(), _serviceProvider)
            ?? throw new InvalidOperationException($"No writer found for type '{payload.LoadTargetConfig.TargetInfo.GetType()}'");

        await writer.WriteAsync(payload.LoadTargetConfig.TargetInfo, payload.Record.GetNormalized(), payload.PipelineId);



    }
}
