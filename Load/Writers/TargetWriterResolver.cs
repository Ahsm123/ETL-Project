using ETL.Domain.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Load.Writers;

public class TargetWriterResolver : ITargetWriterResolver
{
    private readonly Dictionary<string, Type> _map;

    public TargetWriterResolver()
    {
        _map = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(ITargetWriter).IsAssignableFrom(t) && !t.IsAbstract)
            .Select(t => new
            {
                Type = t,
                Attribute = t.GetCustomAttributes(typeof(TargetTypeAttribute), false)
                             .Cast<TargetTypeAttribute>()
                             .FirstOrDefault()
            })
            .Where(x => x.Attribute != null)
            .ToDictionary(x => x.Attribute!.Name.ToLowerInvariant(), x => x.Type);
    }

    public ITargetWriter? Resolve(string targetType, IServiceProvider services)
    {
        return _map.TryGetValue(targetType.ToLowerInvariant(), out var type)
            ? services.GetService(type) as ITargetWriter
            : null;
    }
}

