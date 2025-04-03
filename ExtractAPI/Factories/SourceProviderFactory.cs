using ETL.Domain.Sources;
using ExtractAPI.DataSources;

namespace ExtractAPI.Factories;

public class SourceProviderFactory : ISourceProviderFactory
{
    private readonly IServiceProvider _provider;

    public SourceProviderFactory(IServiceProvider provider)
    {
        _provider = provider;
    }

    public IDataSourceProvider GetProvider(SourceInfoBase sourceInfo)
    {
        return sourceInfo switch
        {
            RestApiSourceInfo => _provider.GetRequiredService<RestApiSourceProvider>(),
            ExcelSourceInfo => _provider.GetRequiredService<ExcelDataSourceProvider>(),
            _ => throw new NotSupportedException($"Unsupported source type: {sourceInfo.GetType().Name}")
        };
    }
}
