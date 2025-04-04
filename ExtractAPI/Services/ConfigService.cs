using ETL.Domain.Config;
using ETL.Domain.Json;
using ExtractAPI.Services;
using System.Text.Json;

public class ConfigService : IConfigService
{
    private readonly HttpClient _httpClient;

    public ConfigService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ConfigFile?> GetByIdAsync(string id)
    {
        var response = await _httpClient.GetAsync($"/api/ConfigFile/{id}");
        if (!response.IsSuccessStatusCode) return null;
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ConfigFile>(json, JsonOptionsFactory.Default);
    }
}