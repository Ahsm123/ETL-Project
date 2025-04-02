using ETL.Domain.Attributes;
using ExtractAPI.DataSources;

namespace ExtractAPI.Utilities;

public class SourceProviderResolver
{
    private static readonly Dictionary<string, Type> _map;
    static SourceProviderResolver()
    {
        _map = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IDataSourceProvider).IsAssignableFrom(t) && !t.IsAbstract)
            .Select(t => new
            {
                Type = t,
                Attribute = t.GetCustomAttributes(typeof(SourceProviderTypeAttribute), false)
                             .Cast<SourceProviderTypeAttribute>()
                             .FirstOrDefault()
            })
            .Where(x => x.Attribute != null)
            .ToDictionary(x => x.Attribute!.Name, x => x.Type);
    }

    public static IDataSourceProvider? Resolve(string sourceType, IServiceProvider services)
    {
        if (_map.TryGetValue(sourceType.ToLowerInvariant(), out var type))
        {
            return services.GetService(type) as IDataSourceProvider;
        }

        return null;
    }
}
