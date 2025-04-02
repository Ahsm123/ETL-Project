using ETL.Domain.Attributes;
using ETL.Domain.Sources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.Domain.Utilities;

public static class SourceTypeMapper
{
    private static readonly Dictionary<string, Type> _map;

    static SourceTypeMapper()
    {
        _map = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(SourceInfoBase).IsAssignableFrom(t) && !t.IsAbstract)
            .Select(t => new
            {
                Type = t,
                Attribute = t.GetCustomAttributes(typeof(SourceTypeAttribute), false)
                             .Cast<SourceTypeAttribute>()
                             .FirstOrDefault()
            })
            .Where(x => x.Attribute != null)
            .ToDictionary(x => x.Attribute!.Name, x => x.Type);
    }

    public static Type? GetSourceInfoType(string sourceType)
    {
        return _map.TryGetValue(sourceType.ToLowerInvariant(), out var type) ? type : null;
    }
}
