using Confluent.Kafka;
using ExtractAPI.Kafka;
using ExtractAPI.Kafka.Interfaces;
using Microsoft.Extensions.Options;

public class KafkaProducer : IKafkaProducer, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaProducer> _logger;

    public KafkaProducer(IOptions<KafkaSettings> options, ILogger<KafkaProducer> logger)
    {
        _logger = logger;
        var settings = options.Value;

        try
        {
            var kafkaConfig = new ProducerConfig
            {
                BootstrapServers = settings.BootstrapServers,
                EnableIdempotence = settings.EnableIdempotence,
                Acks = Acks.All,
                MessageSendMaxRetries = settings.MessageSendMaxRetries,
                RetryBackoffMs = settings.RetryBackoffMs,
                CompressionType = Enum.TryParse<CompressionType>(settings.CompressionType, true, out var compression)
                    ? compression
                    : CompressionType.Snappy
            };

            _producer = new ProducerBuilder<string, string>(kafkaConfig).Build();
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to initialize Kafka producer.");
            throw;
        }
    }

    public async Task PublishAsync(string topic, string key, string jsonPayload)
    {
        try
        {
            var result = await _producer.ProduceAsync(topic, new Message<string, string>
            {
                Key = key,
                Value = jsonPayload
            });

            _logger.LogDebug("Message delivered to {TopicPartitionOffset}", result.TopicPartitionOffset);
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError(ex, "Kafka publish failed: {Reason}", ex.Error.Reason);
            throw;
        }
    }

    public void Dispose()
    {
        _producer?.Dispose();
        _logger.LogDebug("Kafka producer disposed.");
    }
}
