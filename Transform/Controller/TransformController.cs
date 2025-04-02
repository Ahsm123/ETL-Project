
using ETL.Domain.Model.DTOs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Transform.Kafka;
using Transform.Services;

namespace Transform.Controller
{
    public class TransformController : BackgroundService
    {
        private readonly IKafkaConsumer _consumer;
        private readonly IKafkaProducer _producer;
        private readonly ITransformService<string> _transformService;
        private readonly ILogger<TransformController> _logger;

        public TransformController(
                IKafkaConsumer consumer,
                IKafkaProducer producer,
                ITransformService<string> transformService,
                ILogger<TransformController> logger)
        {
            _consumer = consumer;
            _producer = producer;
            _transformService = transformService;
            _logger = logger;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Transform service started");

            await _consumer.ConsumeAsync(stoppingToken, async (string message) =>
            {
                try
                {
                    var payload = JsonSerializer.Deserialize<ExtractedPayload>(message);

                    if (payload is null)
                    {
                        _logger.LogWarning("Received null payload");
                        return; // <<< prevent continuing
                    }

                    var transformed = await _transformService.TransformDataAsync(payload);

                    if (transformed == null)
                    {

                        _logger.LogInformation("Payload with id {id} was filtered out and will not be produced", payload.Id);
                        return; // <<< prevent producing null!
                    }

                    await _producer.ProduceAsync("processedData", Guid.NewGuid().ToString(), transformed);
                    _logger.LogInformation("Transformed and produced payload with id {id}", payload.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error transforming payload");
                }
            });

        }
    }
}






