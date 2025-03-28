using ETL.Domain.Model;
using ETL.Domain.Model.DTOs;
using ExtractAPI.DataSources;
using ExtractAPI.Events;
using ExtractAPI.ExtractedEvents;
using System.Text.Json;

namespace ExtractAPI.Services;

public class ExtractService : IExtractService
{
    private readonly IConfigService _configService;
    private readonly DataSourceFactory _dataSourceFactory;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly FieldFilterService _fieldFilterService;

    public ExtractService(
        IConfigService configService,
        DataSourceFactory dataSourceFactory,
        IEventDispatcher eventDispatcher,
        FieldFilterService fieldFilterService)
    {
        _configService = configService;
        _dataSourceFactory = dataSourceFactory;
        _eventDispatcher = eventDispatcher;
        _fieldFilterService = fieldFilterService;
    }

    public async Task<ConfigFile> ExtractAsync(string configId)
    {
        Console.WriteLine($"Starter extract for configId {configId}");

        // Hent config fra ConfigAPI
        var config = await _configService.GetByIdAsync(configId);
        if (config == null)
        {
            throw new Exception($"Config not found for ID: {configId}");
        }

        Console.WriteLine($"SourceType: {config.SourceType}");

        // Hent den rigtige dataprovider ud fra SourceType-property
        var provider = _dataSourceFactory.GetProvider(config.SourceType);

        // Hent data fra kilde
        var data = await provider.GetDataAsync(config.SourceInfo);

        // Filtrer dataen, hvis det er specificeret i config
        if (config.Extract?.Fields != null && config.Extract.Fields.Any())
        {
            var filteredData = _fieldFilterService.FilterFields(data, config.Extract.Fields);
            config.Data = JsonDocument.Parse(JsonSerializer.Serialize(filteredData)).RootElement;
        }
        else
        {
            config.Data = data;
        }

        // Send beskeder til Kafka - én pr. row i dataen
        if (config.Data.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in config.Data.EnumerateArray())
            {
                var payload = new ExtractedPayload
                {
                    Id = config.Id,
                    SourceType = config.SourceType,
                    Transform = config.Transform,
                    Load = config.Load,
                    Data = item
                };

                var json = JsonSerializer.Serialize(payload);

                await _eventDispatcher.DispatchAsync(new DataExtractedEvent(payload));
            }
        }
        else
        {
            // hvis dataen kun er ét objekt, sendes det som en besked.
            var payload = new ExtractedPayload
            {
                Id = config.Id,
                SourceType = config.SourceType,
                Transform = config.Transform,
                Load = config.Load,
                Data = config.Data
            };

            var json = JsonSerializer.Serialize(payload);
            await _eventDispatcher.DispatchAsync(new DataExtractedEvent(payload));
        }

        Console.WriteLine("Data retrieved:");
        Console.WriteLine(JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true }));

        return config;
    }
}
