using ETL.Domain.Events;
using ETL.Domain.Json;
using Load.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Load.Services;

public class LoadHandler : ILoadHandler
{
    private readonly ITargetWriterResolver _targetWriterResolver;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<LoadHandler> _logger;

    public LoadHandler(
        ITargetWriterResolver targetWriterResolver,
        IServiceProvider serviceProvider,
        ILogger<LoadHandler> logger)
    {
        _targetWriterResolver = targetWriterResolver;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task HandleAsync(string json)
    {
        if (string.IsNullOrWhiteSpace(json) || json == "{}")
        {
            throw new InvalidOperationException("Received empty or invalid payload.");
        }

        var payload = JsonSerializer.Deserialize<TransformedEvent>(json, JsonOptionsFactory.Default);

        if (payload == null)
            throw new InvalidOperationException("Deserialized payload was null.");

        if (payload.LoadTargetConfig?.TargetInfo == null)
            throw new InvalidOperationException("TargetInfo is missing from payload.");

            var targetInfo = payload.LoadTargetConfig.TargetInfo;

        var writer = _targetWriterResolver.Resolve(targetInfo.GetType(), _serviceProvider)
            ?? throw new InvalidOperationException($"No writer found for type '{targetInfo.GetType()}'");

        await writer.WriteAsync(targetInfo, payload.Data);
    }

}
