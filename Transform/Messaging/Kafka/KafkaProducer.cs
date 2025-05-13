using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Transform.Messaging.Interfaces;
using Transform.Messaging.Kafka.KafkaConfig;

public class KafkaProducer : IMessagePublisher, IDisposable
{
    private readonly ILogger<KafkaProducer> _logger;
    private readonly IProducer<string, string> _producer;

    public KafkaProducer(ILogger<KafkaProducer> logger, IOptions<KafkaSettings> options)
    {
        _logger = logger;
        var settings = options.Value;

        var config = new ProducerConfig
        {
            BootstrapServers = settings.BootstrapServers,
            EnableIdempotence = settings.Producer.EnableIdempotence,
            Acks = Enum.TryParse<Acks>(settings.Producer.Acks, true, out var parsed) ? parsed : Acks.All
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task PublishAsync(string topic, string key, string payload)
    {
        try
        {
            var result = await _producer.ProduceAsync(topic, new Message<string, string> { Key = key, Value = payload });
            _logger.LogInformation("Published message to {Topic}", topic);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish to Kafka topic {Topic}", topic);
        }
    }

    public void Dispose()
    {
        try
        {
            _producer.Flush(TimeSpan.FromSeconds(10)); // Flush before dispose
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kafka producer flush failed");
        }

        _producer.Dispose();
    }
}
