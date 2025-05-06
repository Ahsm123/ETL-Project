namespace Load.Messaging.Kafka.KafkaConfig;

public class KafkaSettings
{
    public string BootstrapServers { get; set; }
    public required ConsumerSettings Consumer { get; set; }
    public required ProducerSettings Producer { get; set; }
    public required TopicSettings Topics { get; set; }
}
