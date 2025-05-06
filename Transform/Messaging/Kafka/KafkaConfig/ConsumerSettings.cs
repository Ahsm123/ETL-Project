namespace Transform.Messaging.Kafka.KafkaConfig;

public class ConsumerSettings
{
    public string Topic { get; set; }
    public string GroupId { get; set; }
    public string AutoOffsetReset { get; set; }
}
