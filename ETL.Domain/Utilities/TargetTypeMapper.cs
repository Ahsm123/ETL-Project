using ETL.Domain.Attributes;
using ETL.Domain.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ETL.Domain.Utilities;

public static class TargetTypeMapper
{
    private static readonly Dictionary<string, Type> _map;

    static TargetTypeMapper()
    {
        _map = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(TargetInfoBase).IsAssignableFrom(t) && !t.IsAbstract)
            .Select(t => new
            {
                Type = t,
                Attribute = t.GetCustomAttributes(typeof(TargetTypeAttribute), false)
                             .Cast<TargetTypeAttribute>()
                             .FirstOrDefault()
            })
            .Where(x => x.Attribute != null)
            .ToDictionary(x => x.Attribute!.Name, x => x.Type);
    }

    public static Type? GetTargetInfoType(string targetType)
    {
        return _map.TryGetValue(targetType.ToLowerInvariant(), out var type) ? type : null;
    }
}
