using System.Text.Json;
using FluentAssertions;
using Moq;
using QFace.Sdk.Extensions;
using QimErp.Proxy.Mobile.Shared.Clients;
using QimErp.Proxy.Mobile.WebApi.Features.Auth;

namespace QimErp.Proxy.Mobile.WebApi.Tests;

public class TenantConfigMobileTests
{
    [Fact]
    public async Task Handle_delegates_to_iam_with_domain()
    {
        var iam = new Mock<IIamDownstreamClient>();
        var payload = JsonDocument.Parse(
            """{"company":"Calbank","domain":"calbank","isActive":true}""").RootElement;
        iam.Setup(x => x.GetTenantConfigAsync("calbank", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.WithSuccess(payload));

        var handler = new TenantConfigMobile.Handler(iam.Object);

        var result = await handler.Handle(
            new TenantConfigMobile.Query { Domain = "calbank" },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.GetProperty("company").GetString().Should().Be("Calbank");
        iam.Verify(x => x.GetTenantConfigAsync("calbank", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_forwards_downstream_failure()
    {
        var iam = new Mock<IIamDownstreamClient>();
        iam.Setup(x => x.GetTenantConfigAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.WithFailure<JsonElement>(
                new Error("GetTenantConfig.TenantNotFound", "Tenant not found."), code: "404"));

        var handler = new TenantConfigMobile.Handler(iam.Object);

        var result = await handler.Handle(
            new TenantConfigMobile.Query { Domain = "missing" },
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("GetTenantConfig.TenantNotFound");
    }
}
