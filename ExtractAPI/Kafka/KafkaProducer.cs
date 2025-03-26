
using Confluent.Kafka;

namespace ExtractAPI.Kafka;

public class KafkaProducer : IKafkaProducer
{

    // sender beskeder til Kafka
    private readonly IProducer<string, string> _producer;

    public KafkaProducer(IConfiguration config)
    {
        // bestemmer hvilken Kafka server vi skal forbinde til
        var kafkaConfig = new ProducerConfig
        {
            BootstrapServers = config["Kafka:BootstrapServers"] ?? "localhost:9092"
        };

        // opretter en producer

        _producer = new ProducerBuilder<string, string>(kafkaConfig).Build();
    }

    // sender en besked til et topic
    public async Task PublishAsync(string topic, string key, string jsonPayload)
    {
        var message = new Message<string, string>
        {
            Key = key, // partition key
            Value = jsonPayload // dataen
        };

        await _producer.ProduceAsync(topic, message);
    }
}
