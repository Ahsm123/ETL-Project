using ETL.Domain.Sources;
using ExtractAPI.DataSources.Interfaces;
using ExtractAPI.Factories.Interfaces;

namespace ExtractAPI.Factories;

public class SourceProviderResolver : ISourceProviderResolver
{
    private readonly IEnumerable<IDataSourceProvider> _providers;

    public SourceProviderResolver(IEnumerable<IDataSourceProvider> providers)
    {
        _providers = providers;
    }

    public IDataSourceProvider? Resolve(Type sourceInfoType)
    {
        return _providers.FirstOrDefault(p => p.CanHandle(sourceInfoType));
    }
}
