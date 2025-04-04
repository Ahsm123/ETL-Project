using ETL.Domain.Targets;
using ETL.Domain.Targets.DbTargets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Load.Writers;

public class TargetWriterResolver : ITargetWriterResolver
{
    private readonly IEnumerable<ITargetWriter> _writers;

    public TargetWriterResolver(IEnumerable<ITargetWriter> writers)
    {
        _writers = writers;
    }

    public ITargetWriter? Resolve(Type targetInfoType, IServiceProvider _)
    {
        return _writers.FirstOrDefault(writer => writer.CanHandle(targetInfoType));
    }
}



