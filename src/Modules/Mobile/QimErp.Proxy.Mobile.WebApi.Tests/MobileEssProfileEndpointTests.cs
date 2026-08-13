using System.Text.Json;
using FluentAssertions;
using Moq;
using QFace.Sdk.Extensions;
using QimErp.Proxy.Mobile.Shared.Clients;
using QimErp.Proxy.Mobile.WebApi.Features.Profile;

namespace QimErp.Proxy.Mobile.WebApi.Tests;

public class MobileEssProfileEndpointTests
{
    [Fact]
    public async Task ContactInfo_forwards_body_to_ess_downstream()
    {
        var people = new Mock<IPeopleDownstreamClient>();
        people.Setup(x => x.UpdateEssContactInfoAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.WithSuccess(JsonDocument.Parse("""{"id":"e1"}""").RootElement));

        var handler = new UpdateMobileProfileContactInfo.Handler(people.Object);
        var body = JsonDocument.Parse("""{"mobilePhone":"+233201234567"}""").RootElement.Clone();

        var result = await handler.Handle(
            new UpdateMobileProfileContactInfo.Command { Body = body },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        people.Verify(
            x => x.UpdateEssContactInfoAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SubmitPersonalChange_forwards_body()
    {
        var people = new Mock<IPeopleDownstreamClient>();
        people.Setup(x => x.SubmitEssPersonalChangeRequestAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.WithSuccess(JsonDocument.Parse("""{"requestNumber":"EPD-1"}""").RootElement));

        var handler = new SubmitMobilePersonalChangeRequest.Handler(people.Object);
        var body = JsonDocument.Parse("""{"firstName":"Ama","lastName":"Mensah"}""").RootElement.Clone();

        var result = await handler.Handle(
            new SubmitMobilePersonalChangeRequest.Command { Body = body },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        people.Verify(
            x => x.SubmitEssPersonalChangeRequestAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetMine_forwards_get()
    {
        var people = new Mock<IPeopleDownstreamClient>();
        people.Setup(x => x.GetMyEssPersonalChangeRequestsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.WithSuccess(JsonDocument.Parse("[]").RootElement));

        var handler = new GetMobilePersonalChangeRequestsMine.Handler(people.Object);
        var result = await handler.Handle(new GetMobilePersonalChangeRequestsMine.Query(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        people.Verify(x => x.GetMyEssPersonalChangeRequestsAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
