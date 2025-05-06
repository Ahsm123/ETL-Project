using Confluent.Kafka;
using Load.Interfaces;
using Load.Messaging.Kafka.KafkaConfig;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class KafkaConsumer : IMessageListener
{
    private readonly ILogger<KafkaConsumer> _logger;
    private readonly KafkaSettings _settings;

    public KafkaConsumer(ILogger<KafkaConsumer> logger, IOptions<KafkaSettings> kafkaOptions)
    {
        _logger = logger;
        _settings = kafkaOptions.Value;
    }

    public async Task ListenAsync(Func<string, Task> handleMessage, CancellationToken cancellationToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _settings.BootstrapServers,
            GroupId = _settings.Consumer.GroupId,
            AutoOffsetReset = Enum.Parse<AutoOffsetReset>(_settings.Consumer.AutoOffsetReset, true),
            EnableAutoCommit = false
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();
        consumer.Subscribe(_settings.Consumer.Topic);

        _logger.LogInformation("Subscribed to topic: {Topic}", _settings.Consumer.Topic);

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var result = consumer.Consume(cancellationToken);
                if (!string.IsNullOrEmpty(result?.Message?.Value))
                    await handleMessage(result.Message.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error consuming message");
            }
        }

        consumer.Close();
    }
}
