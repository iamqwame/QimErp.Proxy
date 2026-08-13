using System.Net;
using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.Protected;
using QimErp.Proxy.Mobile.Shared.Clients;
using QimErp.Proxy.Mobile.Shared.Constants;

namespace QimErp.Proxy.Mobile.WebApi.Tests;

public class DownstreamHttpClientHeaderTests
{
    [Fact]
    public async Task GetAsync_forwards_authorization_and_correlation_id()
    {
        HttpRequestMessage? captured = null;
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage request, CancellationToken _) =>
            {
                captured = request;
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(
                        """{"data":{"ok":true},"isSuccess":true,"isFailure":false,"message":"ok","code":"200"}""",
                        Encoding.UTF8,
                        "application/json")
                };
            });

        var factory = new Mock<IHttpClientFactory>();
        factory.Setup(f => f.CreateClient(DownstreamClientNames.Iam))
            .Returns(new HttpClient(handler.Object)
            {
                BaseAddress = new Uri("http://localhost:9050/")
            });

        var accessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext()
        };
        accessor.HttpContext.Request.Headers.Authorization = "Bearer test-token";
        accessor.HttpContext.Request.Headers["X-Correlation-Id"] = "corr-123";

        var client = new IamDownstreamClient(
            factory.Object,
            accessor,
            NullLogger<IamDownstreamClient>.Instance);

        var result = await client.GetMeAsync();

        result.IsSuccess.Should().BeTrue();
        captured.Should().NotBeNull();
        captured!.Headers.Authorization!.Scheme.Should().Be("Bearer");
        captured.Headers.Authorization.Parameter.Should().Be("test-token");
        captured.Headers.GetValues("X-Correlation-Id").Should().ContainSingle("corr-123");
        captured.RequestUri!.ToString().Should().Contain(MobileApiConstants.Downstream.IamMe);
    }
}
