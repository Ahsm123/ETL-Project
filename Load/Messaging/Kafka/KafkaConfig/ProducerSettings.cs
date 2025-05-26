namespace Load.Messaging.Kafka.KafkaConfig;

public class ProducerSettings
{
    public bool EnableIdempotence { get; set; }
    public string Acks { get; set; }
    
}
