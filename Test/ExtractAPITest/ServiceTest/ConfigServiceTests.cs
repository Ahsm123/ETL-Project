using Xunit;
using Moq;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using ETL.Domain.Config;
using ETL.Domain.JsonHelpers;
using ExtractAPI.Interfaces;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using ETL.Tests.Helpers;


namespace Test.ExtractAPITest.ServiceTest;


public class ConfigServiceTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly Mock<ILogger<ConfigService>> _loggerMock;
    private readonly Mock<IJsonService> _jsonServiceMock;
    private readonly HttpClient _httpClient;

    public ConfigServiceTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _loggerMock = new Mock<ILogger<ConfigService>>();
        _jsonServiceMock = new Mock<IJsonService>();

        _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("http://localhost")
        };
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetByIdAsync_InvalidId_ReturnsNull(string id)
    {
        var service = new ConfigService(_httpClient, _loggerMock.Object, _jsonServiceMock.Object);
        var result = await service.GetByIdAsync(id);
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_HttpFails_ReturnsNull()
    {
        var id = "123";
        var response = new HttpResponseMessage(HttpStatusCode.NotFound);

        _httpMessageHandlerMock.SetupSendAsync(response);

        var service = new ConfigService(_httpClient, _loggerMock.Object, _jsonServiceMock.Object);
        var result = await service.GetByIdAsync(id);
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_DeserializationFails_ReturnsNull()
    {
        var id = "123";
        var json = "{}";

        _httpMessageHandlerMock.SetupSendAsync(json);
        _jsonServiceMock.Setup(x => x.Deserialize<ConfigFile>(It.IsAny<Stream>())).Returns((ConfigFile?)null);

        var service = new ConfigService(_httpClient, _loggerMock.Object, _jsonServiceMock.Object);
        var result = await service.GetByIdAsync(id);
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_Success_ReturnsConfigFile()
    {
        var id = "123";
        var json = "{}";
        var configFile = new ConfigFile();

        _httpMessageHandlerMock.SetupSendAsync(json);
        _jsonServiceMock.Setup(x => x.Deserialize<ConfigFile>(It.IsAny<Stream>())).Returns(configFile);

        var service = new ConfigService(_httpClient, _loggerMock.Object, _jsonServiceMock.Object);
        var result = await service.GetByIdAsync(id);
        Assert.NotNull(result);
        Assert.Equal(configFile, result);
    }
}
