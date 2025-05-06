using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Load.Interfaces
{
    public interface IMessagePublisher
    {
        Task PublishAsync(string key, string payload);
    }
}
