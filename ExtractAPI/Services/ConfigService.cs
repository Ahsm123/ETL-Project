using ETL.Domain.Config;
using ExtractAPI.Converters;
using System.Text.Json;

namespace ExtractAPI.Services;

public class ConfigService : IConfigService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _options;

    public ConfigService(HttpClient httpClient, JsonSerializerOptions? options = null)
    {
        _httpClient = httpClient;
        _options = options ?? new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        _options.Converters.Add(new ConfigFileConverter());
    }

    public async Task<ConfigFile?> GetByIdAsync(string id)
    {
        var response = await _httpClient.GetAsync($"/api/ConfigFile/{id}");

        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ConfigFile>(json, _options);
    }
}