namespace ExtractAPI.Kafka.Interfaces;

public interface IMessagePublisher
{
    Task PublishAsync(string topic, string key, string payload);
}
