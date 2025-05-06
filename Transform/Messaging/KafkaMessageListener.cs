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
    private readonly IConfiguration _config;
    private readonly string _topic;
    private readonly string _groupId;
    private readonly AutoOffsetReset _offsetReset;

    public KafkaMessageListener(ILogger<KafkaMessageListener> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
        _topic = _config["Kafka:Consumer:Topic"] ?? "rawData";
        _groupId = _config["Kafka:Consumer:GroupId"] ?? "transform-consumer-group";

        var offset = _config["Kafka:Consumer:AutoOffsetReset"] ?? "Earliest";
        _offsetReset = Enum.TryParse(offset, out AutoOffsetReset parsed) ? parsed : AutoOffsetReset.Earliest;
    }

    public async Task ListenAsync(Func<string, Task> handleMessage, CancellationToken cancellationToken)
    {
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = _config["Kafka:BootstrapServers"]!,
            GroupId = _groupId,
            AutoOffsetReset = _offsetReset
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
        consumer.Subscribe(_topic);

        _logger.LogInformation("Kafka listener subscribed to topic: {Topic}", _topic);

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
