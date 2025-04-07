namespace ExtractAPI.Kafka.Interfaces
{
    public interface IKafkaProducer
    {
        Task PublishAsync(string topic, string key, string jsonPayload);
    }
}
