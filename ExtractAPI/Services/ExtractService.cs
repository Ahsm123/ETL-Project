using ExtractAPI.DataSources;

namespace ExtractAPI.Services
{
    public class ExtractService : IExtractService
    {
        private readonly IConfigService _configService;
        private readonly DataSourceFactory _dataSourceFactory;

        public ExtractService(IConfigService configService, DataSourceFactory dataSourceFactory)
        {
            _configService = configService;
            _dataSourceFactory = dataSourceFactory;
        }

        public async Task ExtractAsync(string configId)
        {
            Console.WriteLine($"Starter extract for configId {configId}");

            var config = await _configService.GetByIdAsync(configId);
            if (config == null)
            {
                Console.WriteLine("Config not found.");
                return;
            }

            Console.WriteLine($"Using source type: {config.SourceType}");


            var provider = _dataSourceFactory.GetProvider(config.SourceType);


            var data = await provider.GetDataAsync(config.SourceInfo);

            Console.WriteLine($"Retrieved data: {config.JsonContent}");

        }
    }
}
