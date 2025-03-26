
using Confluent.Kafka;

namespace ExtractAPI.Kafka;

public class KafkaProducer : IKafkaProducer
{
    private readonly IProducer<string, string> _producer;

    public KafkaProducer(IConfiguration config)
    {
        var kafkaConfig = new ProducerConfig
        {
            BootstrapServers = config["Kafka:BootstrapServers"] ?? "localhost:9092"
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

        await _producer.ProduceAsync(topic, message);
    }
}
