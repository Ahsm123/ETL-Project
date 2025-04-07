namespace ExtractAPI.Kafka;

public interface IKafkaProducer
{
    Task PublishAsync(string topic, string key, string jsonPayload);
}
