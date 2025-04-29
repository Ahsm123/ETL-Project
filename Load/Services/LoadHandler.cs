using ETL.Domain.Events;
using ETL.Domain.JsonHelpers;
using ETL.Domain.NewFolder;
using ETL.Domain.Targets.DbTargets;
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
        _logger.LogWarning("📥 Incoming raw JSON:\n{Json}", json);
        if (string.IsNullOrWhiteSpace(json) || json == "{}")
            throw new InvalidOperationException("Received empty or invalid payload.");

        var payload = _jsonService.Deserialize<TransformedEvent>(json)
            ?? throw new InvalidOperationException("Failed to deserialize payload.");
        _logger.LogWarning("✅ Deserialized table count: {Count}", payload.LoadTargetConfig?.Tables?.Count ?? -1);

        if (payload.LoadTargetConfig?.TargetInfo == null)
            throw new InvalidOperationException("TargetInfo is missing from payload.");

        if (payload.LoadTargetConfig.Tables == null || payload.LoadTargetConfig.Tables.Count == 0)
            throw new InvalidOperationException("Tables are missing or empty in LoadTargetConfig.");

        var writer = _targetWriterResolver.Resolve(payload.LoadTargetConfig.TargetInfo.GetType(), _serviceProvider)
            ?? throw new InvalidOperationException($"No writer found for type '{payload.LoadTargetConfig.TargetInfo.GetType()}'");

        //try catch block -
        //Valider op mod metadata api
        //Mangler http client


        var context = new LoadContext
        {
            TargetInfo = payload.LoadTargetConfig.TargetInfo,
            Data = payload.Record.GetNormalized(),
            PipelineId = payload.PipelineId,
            Tables = payload.LoadTargetConfig.Tables
        };

        await writer.WriteAsync(context);
    }


}

