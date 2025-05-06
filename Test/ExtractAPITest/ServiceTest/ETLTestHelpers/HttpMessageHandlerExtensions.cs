using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;

namespace ETL.Tests.Helpers
{
    public static class HttpMessageHandlerExtensions
    {
        public static void SetupSendAsync(this Mock<HttpMessageHandler> mock, string content, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            mock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(content)
                });
        }

        public static void SetupSendAsync(this Mock<HttpMessageHandler> mock, HttpResponseMessage response)
        {
            mock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);
        }
    }
}
