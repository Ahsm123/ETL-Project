using ETL.Domain.Config;
using ETL.Domain.JsonHelpers;
using ExtractAPI.Interfaces;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace ExtractAPI.Services;

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
        if (string.IsNullOrWhiteSpace(id))
            return null;

        var endpoint = string.Format(ConfigEndpointTemplate, id);

        try
        {
            var response = await _httpClient.GetAsync(endpoint);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch config {ConfigId}. Status code: {StatusCode}", id, response.StatusCode);
                return null;
            }

            await using var stream = await response.Content.ReadAsStreamAsync();
            var config = _jsonService.Deserialize<ConfigFile>(stream);

            if (config == null)
                _logger.LogWarning("Deserialized config for ID {ConfigId} was null.", id);

            return config;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching config {ConfigId}", id);
            return null;
        }
    }
}
