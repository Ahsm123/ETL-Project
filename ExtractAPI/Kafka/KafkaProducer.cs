using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace ExtractAPI.Kafka;

public class KafkaProducer : IKafkaProducer, IDisposable
{
    private readonly IProducer<string, string> _producer;

    public KafkaProducer(IConfiguration config)
    {
        var kafkaConfig = new ProducerConfig
        {
            BootstrapServers = config["Kafka:BootstrapServers"] ?? "localhost:9092",
            EnableIdempotence = true,
            Acks = Acks.All,
            MessageSendMaxRetries = 3,
            RetryBackoffMs = 100,
            CompressionType = CompressionType.Snappy 
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
