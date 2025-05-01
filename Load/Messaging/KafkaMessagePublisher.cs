using Confluent.Kafka;
using Load.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Load.Messaging
{
    public class KafkaMessagePublisher : IMessagePublisher
    {
        private readonly ILogger<KafkaMessagePublisher> _logger;
        private readonly IProducer<string, string> _producer;
        private const string DeadLetterTopic = "dead-letter";

        public KafkaMessagePublisher(ILogger<KafkaMessagePublisher> logger,
            IConfiguration config)
        {
            _logger = logger;

            var enableIdempotence = config.GetValue("Kafka:Producer:EnableIdempotence", true);
            var acks = config["Kafka:Producer:Acks"] ?? "all";

            var producerConfig = new ProducerConfig
            {
                BootstrapServers = "localhost:9092",
                EnableIdempotence = enableIdempotence,
                Acks = Enum.TryParse<Acks>(acks, true, out var ackParsed) ? ackParsed : Acks.All
            };

            _producer = new ProducerBuilder<string, string>(producerConfig).Build();
        }

    public async Task PublishAsync(string key, string payload)
        {
            var message = new Message<string, string> { Key = key, Value = payload };

            try
            {
                var deliveryResult = await _producer.ProduceAsync(DeadLetterTopic, message);

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
                    DeadLetterTopic,
                    key,
                    ex.Error.Reason);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while publishing message to topic '{Topic}' with key '{Key}'",
                    DeadLetterTopic,
                    key);
            }

        }
    }
}
    
    

