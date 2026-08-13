using System.Text.Json;
using FluentAssertions;
using Moq;
using QFace.Sdk.Extensions;
using QimErp.Proxy.Mobile.Shared.Clients;
using QimErp.Proxy.Mobile.WebApi.Features.Auth;

namespace QimErp.Proxy.Mobile.WebApi.Tests;

public class CompleteTwoFactorMobileTests
{
    [Fact]
    public async Task Handle_sends_user_id_and_code_to_iam()
    {
        var iam = new Mock<IIamDownstreamClient>();
        var payload = JsonDocument.Parse(
            """{"token":"at","refreshToken":"rt","userId":"u1","username":"jane"}""").RootElement;
        iam.Setup(x => x.CompleteTwoFactorLoginAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.WithSuccess(payload));

        var handler = new CompleteTwoFactorMobile.Handler(iam.Object);

        var result = await handler.Handle(
            new CompleteTwoFactorMobile.Command { UserId = "u1", Code = "123456" },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.GetProperty("token").GetString().Should().Be("at");
        iam.Verify(
            x => x.CompleteTwoFactorLoginAsync(
                It.Is<object>(b => b.ToString()!.Contains("u1") && b.ToString()!.Contains("123456")),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
