using ETL.Domain.Targets;
using ETL.Domain.Targets.DbTargets;
using ETLDomain.Resolver;
using Load.Interfaces;

namespace Load.Services;

public class TargetWriterResolver : ITargetWriterResolver
{
    private readonly TypeHandlerResolver<ITargetWriter> _resolver;

    public TargetWriterResolver(IEnumerable<ITargetWriter> writers)
    {
        _resolver = new TypeHandlerResolver<ITargetWriter>(
            writers,
            (writer, type) => writer.CanHandle(type)
        );
    }

    public ITargetWriter? Resolve(Type targetInfoType, IServiceProvider _)
    {
        return _resolver.Resolve(targetInfoType);
    }
}



