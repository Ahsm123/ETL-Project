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
                var result = consumer.Consume(cancellationToken);
                _logger.LogInformation("Consumed message: {Key}", result.Message.Key);

                await handleMessage(result.Message.Value);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Kafka listener stopping...");
        }
        finally
        {
            consumer.Close();
        }
    }
}