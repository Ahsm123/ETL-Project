using ETL.Domain.Config;
using ETL.Domain.JsonHelpers;
using ExtractAPI.Interfaces;

public class ConfigService : IConfigService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ConfigService> _logger;
    private readonly IJsonService _jsonService;

    private const string ConfigEndpointTemplate = "/api/Pipeline/{0}";

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
        if (IsInvalidId(id))
            return null;

        var endpoint = FormatEndpoint(id);

        try
        {
            var response = await _httpClient.GetAsync(endpoint);

            if (!response.IsSuccessStatusCode)
                return await HandleFailedResponseAsync(id, response);

            return await DeserializeConfigAsync(response, id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching config {ConfigId}", id);
            return null;
        }
    }

    private static bool IsInvalidId(string id)
    {
        return string.IsNullOrWhiteSpace(id);
    }

    private string FormatEndpoint(string id)
    {
        return string.Format(ConfigEndpointTemplate, id);
    }

    private async Task<ConfigFile?> HandleFailedResponseAsync(string id, HttpResponseMessage response)
    {
        _logger.LogWarning("Failed to fetch config {ConfigId}. Status code: {StatusCode}", id, response.StatusCode);
        return null;
    }

    private async Task<ConfigFile?> DeserializeConfigAsync(HttpResponseMessage response, string id)
    {
        await using var stream = await response.Content.ReadAsStreamAsync();
        var config = _jsonService.Deserialize<ConfigFile>(stream);

        if (config == null)
            _logger.LogWarning("Deserialized config for ID {ConfigId} was null.", id);

        return config;
    }
}
