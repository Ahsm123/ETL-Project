namespace ExtractAPI.Messaging.Kafka.KafkaConfig;

public class KafkaSettings
{
    public string BootstrapServers { get; set; }
    public string Topic { get; set; }
    public bool EnableIdempotence { get; set; }
    public int MessageSendMaxRetries { get; set; }
    public int RetryBackoffMs { get; set; }
    public string CompressionType { get; set; }
}
