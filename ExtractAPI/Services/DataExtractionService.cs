using ETL.Domain.Model;
using ETL.Domain.Model.DTOs;
using ExtractAPI.DataSources;
using ExtractAPI.Events;
using ExtractAPI.ExtractedEvents;
using System.Text.Json;

namespace ExtractAPI.Services
{
    public class DataExtractionService : IDataExtractionService
    {
        private readonly IConfigService _configService;
        private readonly DataSourceProviderFactory _dataSourceFactory;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly DataFieldSelectorService _fieldSelectorService;
        private readonly ILogger<DataExtractionService> _logger;

        public DataExtractionService(
            IConfigService configService,
            DataSourceProviderFactory dataSourceFactory,
            IEventDispatcher eventDispatcher,
            DataFieldSelectorService fieldSelectorService,
            ILogger<DataExtractionService> logger)
        {
            _configService = configService;
            _dataSourceFactory = dataSourceFactory;
            _eventDispatcher = eventDispatcher;
            _fieldSelectorService = fieldSelectorService;
            _logger = logger;
        }

        public async Task<ConfigFile> ExtractAndDispatchAsync(string configId)
        {
            var config = await GetConfigAsync(configId);
            var data = await GetDataAsync(config);
            return await FilterAndDispatchDataAsync(config, data);
        }

        private async Task<ConfigFile> FilterAndDispatchDataAsync(ConfigFile config, JsonElement data)
        {
            // Filtrer dataen hvis der er angivet specifikke felter i config
            if (config.Extract?.Fields != null && config.Extract.Fields.Any())
            {
                var filteredData = _fieldSelectorService.FilterFields(data, config.Extract.Fields);

                // Konverter filtreret data tilbage til JsonElement
                config.Data = JsonDocument.Parse(JsonSerializer.Serialize(filteredData)).RootElement;
            }
            else
            {
                config.Data = data;
            }

            // Send data som events til Kafka
            switch (config.Data)
            {
                case { ValueKind: JsonValueKind.Array }:
                    foreach (var item in config.Data.EnumerateArray())
                    {
                        var payload = CreatePayload(config, item);
                        await _eventDispatcher.DispatchAsync(new DataExtractedEvent(payload));
                    }
                    break;

                case { ValueKind: JsonValueKind.Object }:
                    var singlePayload = CreatePayload(config, config.Data);
                    await _eventDispatcher.DispatchAsync(new DataExtractedEvent(singlePayload));
                    break;

                default:
                    _logger.LogWarning("Unsupported data format in config {ConfigId}", config.Id);
                    break;
            }

            _logger.LogInformation("Data extracted and sent for config {ConfigId}", config.Id);
            return config;
        }

        private ExtractedPayload CreatePayload(ConfigFile config, JsonElement data)
        {
            return new ExtractedPayload
            {
                Id = config.Id,
                SourceType = config.SourceType,
                Transform = config.Transform,
                Load = config.Load,
                Data = data
            };
        }

        private async Task<JsonElement> GetDataAsync(ConfigFile config)
        {
            // Find den rigtige datakilde baseret på SourceType
            var provider = _dataSourceFactory.GetProvider(config.SourceType);

            // Hent data fra kilden
            return await provider.GetDataAsync(config.SourceInfo);
        }

        private async Task<ConfigFile> GetConfigAsync(string configId)
        {
            var config = await _configService.GetByIdAsync(configId);
            if (config == null)
            {
                _logger.LogError("Config not found for ID: {ConfigId}", configId);
                throw new Exception($"Config not found for ID: {configId}");
            }

            return config;
        }
    }
}
