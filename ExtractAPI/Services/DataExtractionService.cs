using ETL.Domain.Config;
using ETL.Domain.Model.DTOs;
using ExtractAPI.DataSources;
using ExtractAPI.Events;
using ExtractAPI.ExtractedEvents;
using System.Text.Json;

namespace ExtractAPI.Services;

public class DataExtractionService : IDataExtractionService
{
    private readonly IConfigService _configService;
    private readonly DataSourceProviderFactory _dataSourceFactory;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly DataFieldSelectorService _selectorService;
    private readonly ILogger<DataExtractionService> _logger;

    public DataExtractionService(
        IConfigService configService,
        DataSourceProviderFactory dataSourceFactory,
        IEventDispatcher eventDispatcher,
        DataFieldSelectorService selectorService,
        ILogger<DataExtractionService> logger)
    {
        _configService = configService;
        _dataSourceFactory = dataSourceFactory;
        _eventDispatcher = eventDispatcher;
        _selectorService = selectorService;
        _logger = logger;
    }

    public async Task<ExtractResponseDto> ExtractAsync(string configId)
    {
        // Hent konfiguration fra API
        var config = await GetConfig(configId);

        // Hent data fra kilde
        var data = await GetData(config);

        // Filtrér og send data til event-system
        var messagesSent = await FilterAndDispatchData(config, data);

        return new ExtractResponseDto
        {
            PipelineId = config.Id,
            MessagesSent = messagesSent
        };
    }

    private async Task<int> FilterAndDispatchData(ConfigFile? config, JsonElement data)
    {
        var dispatchTasks = new List<Task>();

        if (config.Extract?.Fields != null && config.Extract.Fields.Any())
        {
            var filteredData = _selectorService.FilterFields(data, config.Extract.Fields);
            foreach (var item in filteredData)
            {
                dispatchTasks.Add(DispatchPayload(config, item));
            }
        }
        else
        {
            if (data.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in data.EnumerateArray())
                {
                    var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(item.GetRawText());
                    dispatchTasks.Add(DispatchPayload(config, dict!));
                }
            }
            else
            {
                var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(data.GetRawText());
                dispatchTasks.Add(DispatchPayload(config, dict!));
            }
        }

        await Task.WhenAll(dispatchTasks);

        _logger.LogInformation("Pipeline {PipelineId} sendte {Count} beskeder til Kafka", config.Id, dispatchTasks.Count);

        return dispatchTasks.Count;
    }

    private async Task DispatchPayload(ConfigFile config, Dictionary<string, object> data)
    {
        try
        {
            var payload = new ExtractedPayload
            {
                Id = config.Id,
                SourceType = config.SourceType,
                Transform = config.Transform,
                Load = config.Load,
                Data = data
            };

            await _eventDispatcher.DispatchAsync(new DataExtractedEvent(payload));
        }
        catch (Exception ex)
        {
            // Fejllog ved dispatch
            _logger.LogError(ex, "Fejl under dispatch for pipeline {PipelineId}", config.Id);
            throw;
        }
    }

    private async Task<JsonElement> GetData(ConfigFile? config)
    {
        var provider = _dataSourceFactory.GetProvider(config.SourceType);
        var data = await provider.GetDataAsync(config.SourceInfo);
        return data;
    }

    private async Task<ConfigFile?> GetConfig(string configId)
    {
        var config = await _configService.GetByIdAsync(configId);
        if (config == null)
        {
            _logger.LogError("Config blev ikke fundet for ID: {ConfigId}", configId);
            throw new Exception($"Config blev ikke fundet for ID: {configId}");
        }

        return config;
    }
}
