using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transform.Kafka.Interfaces;

namespace Transform.Kafka
{
    public class KafkaConsumer : IKafkaConsumer
    {

        private readonly string _bootstrapServers;
        private readonly string _groupId;
        private readonly string _topic;
        private string bootstrapServers;
        private string groupId;


        public KafkaConsumer(string bootstrapServers, string groupId, string topic)
        {
            _bootstrapServers = bootstrapServers;
            _groupId = groupId;
            _topic = "rawData";
        }   

        public async Task ConsumeAsync(CancellationToken cancellationToken, Func<string, Task> onMessageReceivedAsync)
        {
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = _bootstrapServers,
                GroupId = _groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using (var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build())
            {
                consumer.Subscribe(_topic);

                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        try
                        {
                            var consumeResult = consumer.Consume(cancellationToken);
                            var message = consumeResult.Message.Value;

                            
                           await onMessageReceivedAsync(message);
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
