namespace ExtractAPI.DataSources;

public class DataSourceProviderFactory
{
    private readonly IServiceProvider _serviceProvider;

    public DataSourceProviderFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IDataSourceProvider GetProvider(string sourceType)
    {
        return sourceType.ToLower() switch
        {
            "api" => _serviceProvider.GetRequiredService<ApiDataSourceProvider>(),
            _ => throw new NotSupportedException($"Unknown source type: {sourceType}")
        };
    }
}
