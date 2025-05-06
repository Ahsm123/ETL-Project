namespace Transform.Messaging.Kafka.KafkaConfig;

public class TopicSettings
{
    public bool EnableDeadLetter { get; set; } = true;
    public string DeadLetterTopic { get; set; } = "dead-letter";
}
