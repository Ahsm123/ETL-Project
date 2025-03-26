using ETL.Domain.Model.SourceInfo;
using System.Runtime.InteropServices.Marshalling;
using System.Text.Json;

namespace ExtractAPI.DataSources;

public class ApiDataSourceProvider : IDataSourceProvider
{
    private readonly HttpClient _httpClient;

    public ApiDataSourceProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<JsonElement> GetDataAsync(SourceInfoBase sourceInfo)
    {
        if (sourceInfo is not ApiSourceBaseInfo apiInfo)
            throw new ArgumentException("Invalid source info for API source", nameof(sourceInfo));

        var request = new HttpRequestMessage(HttpMethod.Get, apiInfo.Url);

        if (apiInfo.Headers != null)
        {
            foreach (var header in apiInfo.Headers)
            {
                request.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<JsonElement>(content);
    }
}



