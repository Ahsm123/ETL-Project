using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Extract.DataSources;

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
