using Confluent.Kafka;
using ExtractAPI.Messaging.Interfaces;
using ExtractAPI.Messaging.Kafka.KafkaConfig;
using Microsoft.Extensions.Options;

public class KafkaProducer : IMessagePublisher, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaProducer> _logger;
    private readonly string _defaultTopic;

    public KafkaProducer(IOptions<KafkaSettings> options, ILogger<KafkaProducer> logger)
    {
        _logger = logger;
        var kafkaSettings = options.Value;

        _defaultTopic = kafkaSettings.Topic;

        var config = new ProducerConfig
        {
            BootstrapServers = kafkaSettings.BootstrapServers,
            EnableIdempotence = kafkaSettings.EnableIdempotence,
            Acks = Acks.All,
            MessageSendMaxRetries = kafkaSettings.MessageSendMaxRetries,
            RetryBackoffMs = kafkaSettings.RetryBackoffMs,
            CompressionType = Enum.TryParse<CompressionType>(kafkaSettings.CompressionType, true, out var compression)
                ? compression
                : CompressionType.Snappy
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task PublishAsync(string topic, string key, string payload)
    {
        topic ??= _defaultTopic;

        var message = new Message<string, string> { Key = key, Value = payload };
        await _producer.ProduceAsync(topic, message);

        _logger.LogDebug("Published message to {Topic} with key {Key}", topic, key);
    }

    public void Dispose()
    {
        try
        {
            _producer.Flush(TimeSpan.FromSeconds(10)); // Ensure delivery
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kafka producer flush failed");
        }

        _producer?.Dispose();
    }
}
