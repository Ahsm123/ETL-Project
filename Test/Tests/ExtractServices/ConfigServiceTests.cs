using ETL.Domain.Sources;
using ExtractAPI.Converters;
using ExtractAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Test.Tests.Services;

public class ConfigServiceTests
{
    [Fact]
    public async Task GetByIdAsync_ReturnsConfig_WithApiSourceInfo()
    {
        // Arrange – JSON response from fake Config API
        var jsonResponse = """
        {
            "Id": "pipeline_001",
            "SourceType": "api",
            "SourceInfo": {
                "Url": "https://localhost/api/test",
                "Headers": {}
            },
            "Extract": {
                "Fields": ["account_id", "cost"],
                "Filters": []
            },
            "Transform": {
                "Mappings": [],
                "Filters": []
            },
            "Load": {
                "DestinationType": "database",
                "ConnectionInfo": "Server=db;User Id=admin;Password=secret;",
                "DestinationRessource": "approved_payments"
            }
        }
        """;

        // Mocked HTTP handler that returns the above JSON
        var handler = new MockHttpMessageHandler(jsonResponse, HttpStatusCode.OK);
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://fake-api")
        };

        // Use the same options with ConfigFileConverter
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        options.Converters.Add(new ConfigFileConverter());

        var service = new ConfigService(httpClient, options);

        // Act
        var result = await service.GetByIdAsync("pipeline_001");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("pipeline_001", result!.Id);
        Assert.Equal("api", result.SourceType);
        Assert.IsType<RestApiSourceInfo>(result.SourceInfo);

        var sourceInfo = (RestApiSourceInfo)result.SourceInfo;
        Assert.Equal("https://localhost/api/test", sourceInfo.Url);
        Assert.Empty(sourceInfo.Headers);
    }

    // Simple mock handler to simulate HTTP responses
    private class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly string _response;
        private readonly HttpStatusCode _statusCode;

        public MockHttpMessageHandler(string response, HttpStatusCode statusCode)
        {
            _response = response;
            _statusCode = statusCode;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var httpResponse = new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent(_response, Encoding.UTF8, "application/json")
            };
            return Task.FromResult(httpResponse);
        }
    }
}
