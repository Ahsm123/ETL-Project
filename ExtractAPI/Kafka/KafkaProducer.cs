using Confluent.Kafka;
using ExtractAPI.Kafka.Interfaces;
using Microsoft.Extensions.Options;

namespace ExtractAPI.Kafka;

public class KafkaProducer : IKafkaProducer, IDisposable
{
    private readonly IProducer<string, string> _producer;

    public KafkaProducer(IOptions<KafkaSettings> kafkaSettings)
    {
        var settings = kafkaSettings.Value;

        var kafkaConfig = new ProducerConfig
        {
            BootstrapServers = settings.BootstrapServers,
            EnableIdempotence = settings.EnableIdempotence,
            Acks = Enum.TryParse<Acks>(settings.Acks, true, out var acks) ? acks : Acks.All,
            MessageSendMaxRetries = settings.MessageSendMaxRetries,
            RetryBackoffMs = settings.RetryBackoffMs,
            CompressionType = Enum.TryParse<CompressionType>(settings.CompressionType, true, out var compression) ? compression : CompressionType.Snappy
        };

        _producer = new ProducerBuilder<string, string>(kafkaConfig).Build();
    }

    public async Task PublishAsync(string topic, string key, string jsonPayload)
    {
        var message = new Message<string, string>
        {
            Key = key,
            Value = jsonPayload
        };

        try
        {
            var result = await _producer.ProduceAsync(topic, message);
        }
        catch (ProduceException<string, string> ex)
        {
            throw new Exception($"Kafka publish failed: {ex.Error.Reason}", ex);
        }
    }

    public void Dispose()
    {
        _producer?.Dispose();
    }
}
