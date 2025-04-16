using ETL.Domain.Config;
using ETL.Domain.JsonHelpers;
using ExtractAPI.Interfaces;

public class ConfigService : IConfigService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ConfigService> _logger;
    private readonly IJsonService _jsonService;

    private const string ConfigEndpoint = "/api/Pipeline/{0}";

    public ConfigService(
        HttpClient httpClient,
        ILogger<ConfigService> logger,
        IJsonService jsonService)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonService = jsonService;
    }

    public async Task<ConfigFile?> GetByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            _logger.LogWarning("Attempted to fetch config with empty ID.");
            return null;
        }

        try
        {
            var endpoint = string.Format(ConfigEndpoint, id);
            var response = await _httpClient.GetAsync(endpoint);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch config {ConfigId}. Status code: {StatusCode}", id, response.StatusCode);
                return null;
            }

            await using var stream = await response.Content.ReadAsStreamAsync();
            var config = _jsonService.Deserialize<ConfigFile>(stream);

            if (config == null)
            {
                _logger.LogWarning("Deserialized config for ID {ConfigId} was null.", id);
            }

            return config;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching config {ConfigId}", id);
            return null;
        }
    }
}
