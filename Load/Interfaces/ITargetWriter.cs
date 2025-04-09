using ETL.Domain.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Load.Interfaces;

public interface ITargetWriter
{
    Task WriteAsync(TargetInfoBase targetInfo, Dictionary<string, object> data, string? pipelineId = null);
    bool CanHandle(Type targetInfoType);
}
