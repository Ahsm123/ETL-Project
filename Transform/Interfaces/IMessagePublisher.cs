using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transform.Interfaces
{
    public interface IMessagePublisher
    {
        Task PublishAsync(string topic, string key, string payload);
    }
}
