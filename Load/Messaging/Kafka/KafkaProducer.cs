using Confluent.Kafka;
using Load.Interfaces;
using Load.Messaging.Kafka.KafkaConfig;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class KafkaProducer : IMessagePublisher
{
    private readonly ILogger<KafkaProducer> _logger;
    private readonly IProducer<string, string> _producer;
    private readonly string _deadLetterTopic;

    public KafkaProducer(
        ILogger<KafkaProducer> logger,
        IProducer<string, string> producer,
        IOptions<KafkaSettings> kafkaOptions)
    {
        _logger = logger;
        _producer = producer;
        _deadLetterTopic = kafkaOptions.Value.Topics.DeadLetter;
    }

    public async Task PublishAsync(string key, string payload)
    {
        var message = new Message<string, string> { Key = key, Value = payload };

        try
        {
            var delivery = await _producer.ProduceAsync(_deadLetterTopic, message);
            _logger.LogInformation("Message published to {Topic}", _deadLetterTopic);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish to {Topic}", _deadLetterTopic);
        }
    }
}
