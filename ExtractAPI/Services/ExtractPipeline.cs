using ETL.Domain.Config;
using ETL.Domain.Events;
using ExtractAPI.Interfaces;
using ExtractAPI.Models;
using System.Text.Json;

namespace ExtractAPI.Services;

public class ExtractPipeline : IExtractPipeline
{
    private readonly IConfigService _configService;
    private readonly ISourceProviderResolver _resolver;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly DataFieldSelectorService _selectorService;
    private readonly ILogger<ExtractPipeline> _logger;

    public ExtractPipeline(
        IConfigService configService,
        ISourceProviderResolver resolver,
        IEventDispatcher eventDispatcher,
        DataFieldSelectorService selectorService,
        ILogger<ExtractPipeline> logger)
    {
        _configService = configService;
        _resolver = resolver;
        _eventDispatcher = eventDispatcher;
        _selectorService = selectorService;
        _logger = logger;
    }

    public async Task<ExtractResultEvent> ExtractAsync(string configId)
    {
        var config = await GetConfig(configId);
        var data = await GetData(config);
        var messagesSent = await FilterAndDispatchData(config, data);

        return new ExtractResultEvent
        {
            PipelineId = config.Id,
            MessagesSent = messagesSent
        };
    }

    private async Task<int> FilterAndDispatchData(ConfigFile config, JsonElement data)
    {
        var tasks = new List<Task>();

        var records = config.ExtractConfig?.Fields?.Any() == true
            ? _selectorService.FilterFields(data, config.ExtractConfig.Fields)
            : data.EnumerateArray().Select(item => JsonSerializer.Deserialize<Dictionary<string, object>>(item.GetRawText()));

        foreach (var item in records!)
        {
            tasks.Add(DispatchPayload(config, item!));
        }

        await Task.WhenAll(tasks);
        _logger.LogInformation("Pipeline {PipelineId} sent {Count} messages", config.Id, tasks.Count);
        return tasks.Count;
    }

    private async Task DispatchPayload(ConfigFile config, Dictionary<string, object> data)
    {
        var payload = new ExtractedEvent
        {
            Id = config.Id,
            TransformConfig = config.TransformConfig,
            LoadTargetConfig = config.LoadTargetConfig,
            Data = data
        };

        await _eventDispatcher.DispatchAsync(new DataExtractedEvent(payload));
    }

    private async Task<JsonElement> GetData(ConfigFile config)
    {
        var sourceInfo = config.SourceInfo;

        var provider = _resolver.Resolve(sourceInfo.GetType())
               ?? throw new InvalidOperationException($"No provider found for type {sourceInfo.GetType()}");

        return await provider.GetDataAsync(sourceInfo);
    }



    private async Task<ConfigFile> GetConfig(string configId)
    {
        var config = await _configService.GetByIdAsync(configId);
        if (config == null)
        {
            _logger.LogError("Config not found for ID: {ConfigId}", configId);
            throw new Exception($"Config not found: {configId}");
        }
        return config;
    }
}
