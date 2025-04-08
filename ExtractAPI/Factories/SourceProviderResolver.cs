using ETLDomain.Resolver;
using ExtractAPI.DataSources.Interfaces;
using ExtractAPI.Factories.Interfaces;

public class SourceProviderResolver : ISourceProviderResolver
{
    private readonly TypeHandlerResolver<IDataSourceProvider> _resolver;

    public SourceProviderResolver(IEnumerable<IDataSourceProvider> providers)
    {
        _resolver = new TypeHandlerResolver<IDataSourceProvider>(
            providers,
            (provider, type) => provider.CanHandle(type)
        );
    }

    public IDataSourceProvider? Resolve(Type sourceInfoType)
    {
        return _resolver.Resolve(sourceInfoType);
    }
}