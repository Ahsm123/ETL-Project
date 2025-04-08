using ETL.Domain.Targets;
using ETL.Domain.Targets.DbTargets;
using Load.Interfaces;

namespace Load.Services;

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



