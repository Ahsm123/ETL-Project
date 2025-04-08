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

        await _producer.ProduceAsync(topic, message);

        _logger.LogInformation("Published message to {Topic} with key {Key}", topic, key);
    }
}


