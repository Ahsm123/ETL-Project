using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Transform.Messaging.Interfaces;
using Transform.Messaging.Kafka.KafkaConfig;

public class KafkaConsumer : IMessageListener
{
    private readonly ILogger<KafkaConsumer> _logger;
    private readonly KafkaSettings _settings;

    public KafkaConsumer(ILogger<KafkaConsumer> logger, IOptions<KafkaSettings> options)
    {
        _logger = logger;
        _settings = options.Value;
    }

    public async Task ListenAsync(Func<string, Task> handleMessage, CancellationToken cancellationToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _settings.BootstrapServers,
            GroupId = _settings.Consumer.GroupId,
            AutoOffsetReset = Enum.TryParse(_settings.Consumer.AutoOffsetReset, out AutoOffsetReset parsed)
                ? parsed
                : AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        consumer.Subscribe(_settings.Consumer.Topic);

        _logger.LogInformation("Kafka listener subscribed to topic: {Topic}", _settings.Consumer.Topic);

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var result = consumer.Consume(cancellationToken);
                if (result?.Message == null)
                {
                    _logger.LogWarning("Received null message from Kafka");
                    continue;
                }

                await handleMessage(result.Message.Value);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Kafka listener stopping due to cancellation...");
        }
        finally
        {
            consumer.Close();
            _logger.LogInformation("Kafka consumer closed.");
        }
    }
}
