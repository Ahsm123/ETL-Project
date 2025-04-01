using ExtractAPI.Utilities;

namespace ExtractAPI.DataSources;

public class DataSourceProviderFactory
{
    private readonly IServiceProvider _services;

    public DataSourceProviderFactory(IServiceProvider services)
    {
        _services = services;
    }

    public IDataSourceProvider GetProvider(string sourceType)
    {
        var provider = SourceProviderResolver.Resolve(sourceType, _services);
        if (provider == null)
            throw new NotSupportedException($"Unknown source type: {sourceType}");

        return provider;
    }
}
