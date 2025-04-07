using ETL.Domain.Config;
using ETL.Domain.Json;
using ExtractAPI.Services.Interfaces;
using System.Net.Http.Json;
using System.Text.Json;

public class ConfigService : IConfigService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ConfigService> _logger;

    public ConfigService(HttpClient httpClient, ILogger<ConfigService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
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
            var response = await _httpClient.GetAsync($"/api/ConfigFile/{id}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch config {ConfigId}. Status code: {StatusCode}", id, response.StatusCode);
                return null;
            }

            var stream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<ConfigFile>(stream, JsonOptionsFactory.Default);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching config {ConfigId}", id);
            return null;
        }
    }
}
