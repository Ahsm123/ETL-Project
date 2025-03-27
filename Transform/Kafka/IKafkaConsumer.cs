using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace Transform.Kafka
{
    public interface IKafkaConsumer
    {
        Task ConsumeAsync(string topic,CancellationToken cancellationToken, Action<string> onMessageReceived);
    }
}
