using System.Text.Json;

namespace ExtractAPI.DataSources;

public class ApiDataSourceProvider : IDataSourceProvider
{
    private readonly HttpClient _httpClient;

    public ApiDataSourceProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<JsonElement> GetDataAsync(string url)
    {
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<JsonElement>(content);
    }
}
