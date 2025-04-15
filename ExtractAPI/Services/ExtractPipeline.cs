using ETL.Domain.Config;
using ETL.Domain.Events;
using ExtractAPI.Interfaces;
using ExtractAPI.Models;
using Microsoft.Extensions.Logging;
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

    public async Task<ExtractResultEvent> RunPipelineAsync(string configId)
    {
        var config = await LoadPipelineConfigurationAsync(configId);
        var rawData = await ExtractRawDataAsync(config);
        var recordCount = await ProcessAndDispatchAsync(config, rawData);

        return new ExtractResultEvent
        {
            PipelineId = config.Id,
            MessagesSent = recordCount
        };
    }

    private async Task<ConfigFile> LoadPipelineConfigurationAsync(string configId)
    {
        var config = await _configService.GetByIdAsync(configId);
        if (config == null)
        {
            _logger.LogError("Config not found for ID: {ConfigId}", configId);
            throw new Exception($"Config not found: {configId}");
        }
        return config;
    }

    private async Task<JsonElement> ExtractRawDataAsync(ConfigFile config)
    {
        var sourceInfo = config.ExtractConfig.SourceInfo;

        var provider = _resolver.Resolve(sourceInfo.GetType())
               ?? throw new InvalidOperationException($"No provider found for type {sourceInfo.GetType()}");

        return await provider.GetDataAsync(config.ExtractConfig);
    }

    private async Task<int> ProcessAndDispatchAsync(ConfigFile config, JsonElement rawData)
    {
        var records = SelectRecords(rawData, config);
        var tasks = records.Select(record => DispatchExtractedEventAsync(config, record)).ToList();

        await Task.WhenAll(tasks);
        _logger.LogInformation("Pipeline {PipelineId} sent {Count} messages", config.Id, tasks.Count);

        return tasks.Count;
    }

    private IEnumerable<Dictionary<string, object>> SelectRecords(JsonElement rawData, ConfigFile config)
    {
        var hasFieldSelection = config.ExtractConfig?.Fields?.Any() == true;

        return hasFieldSelection
            ? _selectorService.FilterFields(rawData, config.ExtractConfig.Fields)
            : SafeDeserializeRecords(rawData);
    }

    private IEnumerable<Dictionary<string, object>> SafeDeserializeRecords(JsonElement array)
    {
        foreach (var element in array.EnumerateArray())
        {
            Dictionary<string, object>? parsed = null;
            try
            {
                parsed = JsonSerializer.Deserialize<Dictionary<string, object>>(element.GetRawText());
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Failed to deserialize element: {Element}", element.GetRawText());
            }

            if (parsed != null)
                yield return parsed;
            else
                _logger.LogWarning("Skipping null or invalid record in raw data array");
        }
    }


    private async Task DispatchExtractedEventAsync(ConfigFile config, Dictionary<string, object> record)
    {
        var extractedEvent = new ExtractedEvent
        {
            Id = config.Id,
            TransformConfig = config.TransformConfig,
            LoadTargetConfig = config.LoadTargetConfig,
            Data = record
        };

        await _eventDispatcher.DispatchAsync(extractedEvent);

    }

}
