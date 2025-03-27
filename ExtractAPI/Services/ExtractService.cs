using ETL.Domain.Model;
using ETL.Domain.Model.DTOs;
using ExtractAPI.DataSources;
using ExtractAPI.Kafka;
using System.Text.Json;

namespace ExtractAPI.Services
{
    public class ExtractService : IExtractService
    {
        private readonly IConfigService _configService;
        private readonly DataSourceFactory _dataSourceFactory;
        private readonly IKafkaProducer _kafkaProducer;

        public ExtractService(
            IConfigService configService,
            DataSourceFactory dataSourceFactory,
            IKafkaProducer kafkaProducer)
        {
            _configService = configService;
            _dataSourceFactory = dataSourceFactory;
            _kafkaProducer = kafkaProducer;
        }

        //ExtractService henter data fra en kilde (fx api) baseret på config
        //og sender resultatet videre til et Kafka topic.

        public async Task<ConfigFile> ExtractAsync(string configId)
        {
            Console.WriteLine($"Starter extract for configId {configId}");

            // henter config ud fra pipeline id (fx "pipeline_001") som blev postet til /api/extract/{configId}
            var config = await _configService.GetByIdAsync(configId);
            if (config == null)
            {
                throw new Exception($"Config not found for ID: {configId}");
            }

            Console.WriteLine($"SourceType: {config.SourceType}");

            // returnerer den rigtige provider baseret på sourceType "api", "file" eller "database"
            // da vi har en api source, så returnerer den ApiDataSourceProvider, som er en IDataSourceProvider 
            // og har en metode GetDataAsync, som henter data fra et api
            var provider = _dataSourceFactory.GetProvider(config.SourceType);

            // henter data fra api
            var data = await provider.GetDataAsync(config.SourceInfo);

            // filtrer felter hvis der er angivet i config filen
            // gemmer den hentede data i config objektet
            if (config.Extract?.Fields != null && config.Extract.Fields.Any())
            {
                var filteredData = FilterFields(data, config.Extract.Fields);
                config.Data = JsonDocument.Parse(JsonSerializer.Serialize(filteredData)).RootElement;
            }
            else
            {
                config.Data = data;
            }

           
           

            // mapper config objektet til en ExtractedPayload, som er en DTO, der bruges til at sende data til Kafka
            // hvor vi har fjernet nogle properties, som ikke er nødvendige at sende med: ExtractSettings og SourceInfo

            var payload = MapToKafka(config);
            var json = JsonSerializer.Serialize(payload);

            // sender data og config til Kafka, (specifikt rawData topic), som er defineret i appsettings.json
            // TODO: overvej at bruge en random genereret key i stedet for config.Id mht. load balancing
            await _kafkaProducer.PublishAsync("rawData", config.Id, json);

            Console.WriteLine("Data retrieved:");
            Console.WriteLine(JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true }));

            return config;

        }

        private IEnumerable<Dictionary<string, object>> FilterFields(JsonElement data, List<string> fields)
        {
            var result = new List<Dictionary<string, object>>();

            foreach (var item in data.EnumerateArray())
            {
                var filteredItem = new Dictionary<string, object>();

                foreach (var field in fields)
                {
                    if (item.TryGetProperty(field, out var value))
                    {
                        filteredItem[field] = value.ValueKind switch
                        {
                            JsonValueKind.Number => value.GetDouble(),
                            JsonValueKind.String => value.GetString(),
                            JsonValueKind.True => true,
                            JsonValueKind.False => false,
                            _ => value.ToString()
                        };
                    }
                }

                result.Add(filteredItem);
            }

            return result;
        }


        // metoder der kun mapper det nødvendige information fra ConfigFile til ExtractedPayload
        private ExtractedPayload MapToKafka(ConfigFile config)
        {
            return new ExtractedPayload
            {
                Id = config.Id,
                SourceType = config.SourceType,
                Transform = config.Transform,
                Load = config.Load,
                Data = config.Data!.Value,
            };
        }
    }
}
