using Confluent.Kafka;
using Load.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Load.Messaging;

public class KafkaMessageListener : IMessageListener
{
    private readonly ILogger<KafkaMessageListener> _logger;
    private readonly IConfiguration _configuration;

    public KafkaMessageListener(ILogger<KafkaMessageListener> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task ListenAsync(Func<string, Task> handleMessage, CancellationToken cancellationToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _configuration["Kafka:BootstrapServers"] ?? "localhost:9092",
            GroupId = "load-consumer-group",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();
        consumer.Subscribe("processedData");

        _logger.LogInformation("Subscribed to topic: processedData");

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(cancellationToken);
                    _logger.LogInformation("Consumed message with key: {Key}", result?.Message?.Key);

                    if (!string.IsNullOrEmpty(result?.Message?.Value))
                    {
                        await handleMessage(result.Message.Value);
                    }
                    else
                    {
                        _logger.LogWarning("Received null or empty message.");
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Kafka consume error.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error while handling message.");
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Kafka consumer shutting down...");
        }
        finally
        {
            consumer.Close();
            _logger.LogInformation("Consumer closed.");
        }
    }
}
