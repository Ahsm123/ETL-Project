using ExtractAPI.DataSources;
using ETL.Domain.Model;
using System.Text.Json;
using ExtractAPI.Kafka;
using ETL.Domain.Model.DTOs;

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

        public async Task<ConfigFile> ExtractAsync(string configId)
        {
            Console.WriteLine($"Starter extract for configId {configId}");

            var config = await _configService.GetByIdAsync(configId);
            if (config == null)
            {
                throw new Exception($"Config not found for ID: {configId}");
            }

            Console.WriteLine($"SourceType: {config.SourceType}");

            var provider = _dataSourceFactory.GetProvider(config.SourceType);
            var data = await provider.GetDataAsync(config.SourceInfo);
            config.Data = data;

            var payload = MapToKafka(config);
            var json = JsonSerializer.Serialize(payload);

            await _kafkaProducer.PublishAsync("rawData", config.Id, json);

            Console.WriteLine("Data retrieved:");
            Console.WriteLine(JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true }));

            return config;

        }

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
