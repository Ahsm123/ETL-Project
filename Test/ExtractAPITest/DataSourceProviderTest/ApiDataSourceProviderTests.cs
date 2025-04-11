using ETL.Domain.Model;
using ETL.Domain.Sources;
using ExtractAPI.DataSources;
using Moq.Protected;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Test.ExtractAPITest.DataSourceProviderTest;

public class ApiDataSourceProviderTests
{
    [Fact]
    public async Task GetDataAsync_ShouldReturnJsonElement_WhenResponseIsValid()
    {
        // Arrange
        var expectedJson = "{\"message\":\"Hello World\"}";
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(expectedJson, Encoding.UTF8, "application/json")
        };

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
           .Protected()
           .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
           .ReturnsAsync(httpResponse);

        var httpClient = new HttpClient(handlerMock.Object);

        var provider = new RestApiSourceProvider(httpClient);

        var config = new ExtractConfig
        {
            SourceInfo = new RestApiSourceInfo
            {
                Url = "https://api.test.com/data"
            }
        };

        // Act
        var result = await provider.GetDataAsync(config);

        // Assert
        Assert.Equal("Hello World", result.GetProperty("message").GetString());
    }

    [Fact]
    public async Task GetDataAsync_ShouldThrow_WhenResponseIsError()
    {
        // Arrange
        var httpResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("Bad request")
        };

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
           .Protected()
           .Setup<Task<HttpResponseMessage>>("SendAsync",
               ItExpr.IsAny<HttpRequestMessage>(),
               ItExpr.IsAny<CancellationToken>())
           .ReturnsAsync(httpResponse);

        var httpClient = new HttpClient(handlerMock.Object);
        var provider = new RestApiSourceProvider(httpClient);

        var config = new ExtractConfig
        {
            SourceInfo = new RestApiSourceInfo
            {
                Url = "https://api.test.com/data"
            }
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<HttpRequestException>(() => provider.GetDataAsync(config));
        Assert.Contains("Fejl ved API-kald", ex.Message);
    }
}
