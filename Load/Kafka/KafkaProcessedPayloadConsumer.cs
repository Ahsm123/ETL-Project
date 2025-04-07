using Confluent.Kafka;
using Load.Kafka.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Load.Kafka
{
    public class KafkaProcessedPayloadConsumer : IKafkaConsumer
    {
        private readonly ILogger<KafkaProcessedPayloadConsumer> _logger;
        private readonly IConfiguration _configuration;

        public KafkaProcessedPayloadConsumer(ILogger<KafkaProcessedPayloadConsumer> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task StartAsync(Func<string, Task> handleMessage, CancellationToken cancellationToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _configuration["Kafka:BootstrapServers"] ?? "localhost:9092",
                GroupId = "load-consumer-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<string, string>(config).Build();
            consumer.Subscribe("processedData");

            _logger.LogInformation("Kafka consumer subscribed to topic: processedData");

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var result = consumer.Consume(cancellationToken);

                    _logger.LogInformation("Consumed message with key: {Key}", result.Message.Key);

                    await handleMessage(result.Message.Value);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Kafka consumer shutting down...");
            }
            finally
            {
                consumer.Close();
            }
        }
    }
}
