﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Load.Messaging.Interfaces
{
    public interface IMessagePublisher
    {
        Task PublishAsync(string key, string payload);
    }
}
