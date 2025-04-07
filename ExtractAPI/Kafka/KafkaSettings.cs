namespace ExtractAPI.Kafka
{
    public class KafkaSettings
    {
        public string BootstrapServers { get; set; }
        public bool EnableIdempotence { get; set; }
        public string Acks { get; set; }
        public int MessageSendMaxRetries { get; set; }
        public int RetryBackoffMs { get; set; }
        public string CompressionType { get; set; }
    }

}
