using Confluent.Kafka;
using Load.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                var result = consumer.Consume(cancellationToken);
                _logger.LogInformation("Consumed message with key: {key}", result.Message.Key);

                await handleMessage(result.Message.Value);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Operation canceled");
        }
        finally
        {
            consumer.Close();
            _logger.LogInformation("Consumer closed");
        }


    }
}