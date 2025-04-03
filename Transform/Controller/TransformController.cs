
using ETL.Domain.Events;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Transform.Converters;
using Transform.Kafka;
using Transform.Services;

namespace Transform.Controller;

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
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                };
                options.Converters.Add(new ExtractedEventConverter());

                var payload = JsonSerializer.Deserialize<ExtractedEvent>(message, options);

                if (payload is null)
                {
                    _logger.LogWarning("Received null payload");
                    return;
                }

                    var transformed = await _transformService.TransformDataAsync(payload);
                    await _producer.ProduceAsync("processedData", Guid.NewGuid().ToString(), transformed);

                    _logger.LogInformation("Transformed payload with id {id}", payload.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error transforming payload");
                }
            });


        }
    }
}





