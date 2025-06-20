﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace Transform.Messaging.Interfaces;

public interface IMessageListener
{
    Task ListenAsync(Func<string, Task> handleMessage, CancellationToken cancellationToken);
}
