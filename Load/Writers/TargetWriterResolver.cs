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
    private readonly Dictionary<Type, Type> _map;

    public TargetWriterResolver()
    {
        _map = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(ITargetWriter).IsAssignableFrom(t) && !t.IsAbstract)
            .ToDictionary(
                writerType => GetHandledTargetType(writerType),
                writerType => writerType
            );
    }

    public ITargetWriter? Resolve(Type targetInfoType, IServiceProvider services)
    {
        foreach (var kv in _map)
        {
            if (kv.Key.IsAssignableFrom(targetInfoType))
            {
                return services.GetService(kv.Value) as ITargetWriter;
            }
        }

        return null;
    }

    private static Type GetHandledTargetType(Type writerType)
    {
        // Default to MsSqlTargetInfo or base type
        // You can replace this with attribute-based config if needed
        if (writerType == typeof(MsSqlTargetWriter))
            return typeof(MsSqlTargetInfo);

        return typeof(TargetInfoBase);
    }
}


