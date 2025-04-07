using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Load.Kafka.Interfaces;
public interface IKafkaConsumer
{
    Task StartAsync(Func<string, Task> handleMessage, CancellationToken cancellationToken);
}
