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

public class KafkaMessagePublisher : IMessagePublisher
{
    private readonly ILogger<KafkaMessagePublisher> _logger;
    private readonly IProducer<string, string> _producer;

    public KafkaMessagePublisher(ILogger<KafkaMessagePublisher> logger, IConfiguration config)
    {
        _logger = logger;

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = config["Kafka:BootstrapServers"] ?? "localhost:9092",
            EnableIdempotence = true,
            Acks = Acks.All
        };

        _producer = new ProducerBuilder<string, string>(producerConfig).Build();
    }

    public async Task PublishAsync(string topic, string key, string payload)
    {
        var message = new Message<string, string> { Key = key, Value = payload };

        try
        {
            var deliveryResult = await _producer.ProduceAsync(topic, message);

            _logger.LogInformation(
                "Published message to topic '{Topic}' | Partition: {Partition}, Offset: {Offset}, Key: {Key}",
                deliveryResult.Topic,
                deliveryResult.Partition,
                deliveryResult.Offset,
                key);
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError(
                ex,
                "Failed to publish message to topic '{Topic}' with key '{Key}'. Error: {Reason}",
                topic,
                key,
                ex.Error.Reason);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error while publishing message to topic '{Topic}' with key '{Key}'",
                topic,
                key);
        }
    }
}


