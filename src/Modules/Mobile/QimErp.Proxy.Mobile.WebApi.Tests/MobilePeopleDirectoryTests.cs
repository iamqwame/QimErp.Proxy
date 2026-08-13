using System.Text.Json;
using FluentAssertions;
using Moq;
using QFace.Sdk.Extensions;
using QimErp.Proxy.Mobile.Shared.Clients;
using QimErp.Proxy.Mobile.WebApi.Features.People;

namespace QimErp.Proxy.Mobile.WebApi.Tests;

public class MobilePeopleDirectoryTests
{
    [Fact]
    public async Task Directory_forwards_search_body()
    {
        var people = new Mock<IPeopleDownstreamClient>();
        people.Setup(x => x.GetDirectoryAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.WithSuccess(JsonDocument.Parse("""{"items":[],"totalCount":0}""").RootElement));

        var handler = new GetMobilePeopleDirectory.Handler(people.Object);

        var result = await handler.Handle(
            new GetMobilePeopleDirectory.Command
            {
                Body = JsonDocument.Parse(
                    """{"pageNumber":1,"pageSize":50,"searchTerm":"ada","filter":"all-employees"}""").RootElement
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        people.Verify(
            x => x.GetDirectoryAsync(
                It.Is<object>(b => b.ToString()!.Contains("ada")),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Filters_delegates()
    {
        var people = new Mock<IPeopleDownstreamClient>();
        people.Setup(x => x.GetFiltersAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.WithSuccess(JsonDocument.Parse("""{"filters":[]}""").RootElement));

        var handler = new GetMobilePeopleFilters.Handler(people.Object);

        var result = await handler.Handle(new GetMobilePeopleFilters.Query(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        people.Verify(x => x.GetFiltersAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
