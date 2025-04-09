using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transform.Interfaces;

namespace Transform.Messaging;

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
            GroupId = "transform-consumer-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        consumer.Subscribe("rawData");

        _logger.LogInformation("Kafka listener subscribed to topic: rawData");

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(cancellationToken);

                    if (result?.Message == null)
                    {
                        _logger.LogWarning("Received null message from Kafka");
                        continue;
                    }

                    _logger.LogDebug("Consumed message with key: {Key}", result.Message.Key);

                    try
                    {
                        await handleMessage(result.Message.Value);
                    }
                    catch (Exception handleEx)
                    {
                        _logger.LogError(handleEx, "Error while handling Kafka message: {Value}", result.Message.Value);
                    }
                }
                catch (ConsumeException consumeEx)
                {
                    _logger.LogError(consumeEx, "Kafka consumption error: {Reason}", consumeEx.Error.Reason);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error during Kafka consumption");
                }
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