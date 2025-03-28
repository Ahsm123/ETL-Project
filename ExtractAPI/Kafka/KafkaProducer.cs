
using Confluent.Kafka;

namespace ExtractAPI.Kafka;

public class KafkaProducer : IKafkaProducer
{
    private readonly IProducer<string, string> _producer;

    public KafkaProducer(IConfiguration config)
    {
        // Konfigurer Kafka-producer
        var kafkaConfig = new ProducerConfig
        {
            // Adresse til Kafka-broker
            BootstrapServers = config["Kafka:BootstrapServers"] ?? "localhost:9092",
            // Gør at beskeder bliver sendt 'Idempotent' - ingen dubletter ved retry
            EnableIdempotence = true,           
            Acks = Acks.All,                    
            MessageSendMaxRetries = 3,          
            RetryBackoffMs = 100               
        };

        _producer = new ProducerBuilder<string, string>(kafkaConfig).Build();
    }

    /// Sender en besked til et Kafka-topic med angivet nøgle og JSON-payload.
    /// <param name="topic">Navnet på Kafka-topic'en</param>
    /// <param name="key">Partitioneringsnøgle – bruges til at styre hvilken partition beskeden lander i</param>
    /// <param name="jsonPayload">Selve beskeden som JSON-string</param>
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
