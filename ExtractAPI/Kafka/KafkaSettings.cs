namespace ExtractAPI.Kafka;

public class KafkaSettings
{
    public string BootstrapServers { get; set; } = "localhost:9092";
    public string RawDataTopic { get; set; } = "rawData";
    public bool EnableIdempotence { get; set; } = true;
    public int MessageSendMaxRetries { get; set; } = 3;
    public int RetryBackoffMs { get; set; } = 100;
    public string CompressionType { get; set; } = "snappy";
}
