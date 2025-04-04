using System.Text.Json;
using ETL.Domain.Sources;

namespace ExtractAPI.DataSources;

public class RestApiSourceProvider : IDataSourceProvider
{
    private readonly HttpClient _httpClient;

    public RestApiSourceProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public bool CanHandle(Type sourceInfoType)
    {
        return sourceInfoType == typeof(RestApiSourceInfo);
    }

    public async Task<JsonElement> GetDataAsync(SourceInfoBase sourceInfo)
    {
        if (sourceInfo is not RestApiSourceInfo apiInfo)
            throw new ArgumentException("SourceInfo skal være af typen ApiSourceBaseInfo", nameof(sourceInfo));

        using var request = new HttpRequestMessage(HttpMethod.Get, apiInfo.Url);

        foreach (var header in apiInfo.Headers ?? Enumerable.Empty<KeyValuePair<string, string>>())
        {
            request.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Fejl ved API-kald til {apiInfo.Url}: {response.StatusCode} - {errorBody}");
        }

        using var stream = await response.Content.ReadAsStreamAsync();
        using var document = await JsonDocument.ParseAsync(stream);
        return document.RootElement.Clone();
    }
}
