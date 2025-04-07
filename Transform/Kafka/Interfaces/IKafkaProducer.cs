using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transform.Kafka.Interfaces
{
    public interface IKafkaProducer
    {
        Task ProduceAsync(string topic, string key, string jsonPayload);
    }
}
