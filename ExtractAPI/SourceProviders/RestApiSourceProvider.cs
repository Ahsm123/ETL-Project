using ETL.Domain.Model;
using ETL.Domain.Sources;
using ExtractAPI.SourceProviders.Interfaces;
using System.Text.Json;

namespace ExtractAPI.DataSources;

public class RestApiSourceProvider : IDataSourceProvider
{
    private readonly HttpClient _httpClient;

    public RestApiSourceProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public bool CanHandle(Type sourceInfoType)
        => sourceInfoType == typeof(RestApiSourceInfo);

    public async Task<JsonElement> GetDataAsync(ExtractConfig extractConfig)
    {
        var apiInfo = ValidateSourceInfo(extractConfig);
        using var request = CreateHttpRequest(apiInfo);
        using var response = await SendRequestAndEnsureSuccess(request, apiInfo.Url);
        return await ParseJsonFromResponse(response);
    }

    private RestApiSourceInfo ValidateSourceInfo(ExtractConfig config)
    {
        if (config.SourceInfo is not RestApiSourceInfo apiInfo)
            throw new ArgumentException("SourceInfo skal være af typen ApiSourceBaseInfo", nameof(config.SourceInfo));
        return apiInfo;
    }

    private HttpRequestMessage CreateHttpRequest(RestApiSourceInfo apiInfo)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, apiInfo.Url);
        foreach (var header in apiInfo.Headers ?? Enumerable.Empty<KeyValuePair<string, string>>())
        {
            request.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }
        return request;
    }

    private async Task<HttpResponseMessage> SendRequestAndEnsureSuccess(HttpRequestMessage request, string url)
    {
        var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Fejl ved API-kald til {url}: {response.StatusCode} - {errorBody}");
        }

        return response;
    }

    private async Task<JsonElement> ParseJsonFromResponse(HttpResponseMessage response)
    {
        using var stream = await response.Content.ReadAsStreamAsync();
        using var document = await JsonDocument.ParseAsync(stream);
        return document.RootElement.Clone();
    }
}
