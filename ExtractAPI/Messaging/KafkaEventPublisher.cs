using Confluent.Kafka;
using ExtractAPI.Interfaces;
using ExtractAPI.Kafka;
using ExtractAPI.Kafka.Interfaces;
using Microsoft.Extensions.Options;

public class KafkaEventPublisher : IMessagePublisher, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaEventPublisher> _logger;
    private readonly string _defaultTopic;

    public KafkaEventPublisher(IConfiguration config, ILogger<KafkaEventPublisher> logger)
    {
        _logger = logger;

        var bootstrapServers = config["Kafka:BootstrapServers"] ?? "localhost:9092";
        _defaultTopic = config["Kafka:RawDataTopic"] ?? "rawData";

        var kafkaConfig = new ProducerConfig
        {
            BootstrapServers = bootstrapServers,
            EnableIdempotence = bool.TryParse(config["Kafka:EnableIdempotence"], out var idemp) && idemp,
            Acks = Acks.All,
            MessageSendMaxRetries = int.TryParse(config["Kafka:MessageSendMaxRetries"], out var retries) ? retries : 3,
            RetryBackoffMs = int.TryParse(config["Kafka:RetryBackoffMs"], out var retryMs) ? retryMs : 100,
            CompressionType = Enum.TryParse<CompressionType>(config["Kafka:CompressionType"], true, out var compression)
                ? compression
                : CompressionType.Snappy
        };

        _producer = new ProducerBuilder<string, string>(kafkaConfig).Build();
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
        _producer?.Dispose();
    }
}
