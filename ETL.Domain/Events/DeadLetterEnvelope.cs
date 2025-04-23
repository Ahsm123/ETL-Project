using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.Domain.Events;

public class DeadLetterEnvelope
{
    public string OriginalPayload { get; set; } = string.Empty;
    public string Error { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
