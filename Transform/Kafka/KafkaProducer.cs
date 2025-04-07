using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transform.Kafka.Interfaces;

namespace Transform.Kafka
{
    public class KafkaProducer : IKafkaProducer
        {
            private readonly IProducer<string, string> _producer;
            public KafkaProducer(IConfiguration config)
            {
                // Konfigurer Kafka-producer
                var kafkaConfig = new ProducerConfig
                {
                    // Adresse til Kafka-broker
                    BootstrapServers = config["Kafka:BootstrapServers"] ?? "localhost:9092",
                    // Gør at beskeder bliver sendt 'Idempotent' - ingen dubletter ved retry
                    EnableIdempotence = true,
                    Acks = Acks.All,
                    MessageSendMaxRetries = 3,
                    RetryBackoffMs = 100
                };

                _producer = new ProducerBuilder<string, string>(kafkaConfig).Build();
            }

            public async Task ProduceAsync(string topic, string key, string jsonPayload)
            {
                var message = new Message<string, string>
                {
                    Key = key,
                    Value = jsonPayload
                };

                await _producer.ProduceAsync(topic, message);
            }
        }
    }


