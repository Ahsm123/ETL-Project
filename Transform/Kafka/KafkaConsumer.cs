using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transform.Kafka
{
    public class KafkaConsumer : IKafkaConsumer
    {

        private readonly string _bootstrapServers;
        private readonly string _groupId;

        public KafkaConsumer(string bootstrapServers, string groupId)
        {
            _bootstrapServers = bootstrapServers;
            _groupId = groupId;
        }

        public async Task ConsumeAsync(string consumeTopic, CancellationToken cancellationToken, Action<string> onMessageReceived)
        {
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = _bootstrapServers,
                GroupId = _groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using (var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build())
            {
                consumer.Subscribe(consumeTopic);

                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        try
                        {
                            var consumeResult = consumer.Consume(cancellationToken);
                            var message = consumeResult.Message.Value;

                            
                            onMessageReceived(message);
                        }
                        catch (ConsumeException e)
                        {
                            Console.WriteLine($"Error consuming message: {e.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Kafka consumption canceled.");
                }
                finally
                {
                    consumer.Close();
                }
            }
        }

    }
}
