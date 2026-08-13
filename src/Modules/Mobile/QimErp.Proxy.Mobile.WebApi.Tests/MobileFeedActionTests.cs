using System.Text.Json;
using FluentAssertions;
using Moq;
using QFace.Sdk.Extensions;
using QimErp.Proxy.Mobile.Shared.Clients;
using QimErp.Proxy.Mobile.WebApi.Features.Feed;

namespace QimErp.Proxy.Mobile.WebApi.Tests;

public class MobileFeedActionTests
{
    private static readonly Guid NewsId = Guid.NewGuid();

    [Fact]
    public async Task MarkRead_forwards_newsId()
    {
        var people = new Mock<IPeopleDownstreamClient>();
        people.Setup(x => x.MarkNewsReadAsync(NewsId, It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.WithSuccess(JsonDocument.Parse("""true""").RootElement));

        var handler = new MobileFeedActions.MarkReadHandler(people.Object);

        var result = await handler.Handle(new MobileFeedActions.MarkReadQuery(NewsId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        people.Verify(
            x => x.MarkNewsReadAsync(NewsId, It.IsAny<object>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ToggleReaction_forwards_key()
    {
        var people = new Mock<IPeopleDownstreamClient>();
        people.Setup(x => x.ToggleNewsReactionAsync(NewsId, It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.WithSuccess(JsonDocument.Parse("""{"total":1}""").RootElement));

        var handler = new MobileFeedActions.ToggleReactionHandler(people.Object);

        var result = await handler.Handle(
            new MobileFeedActions.ToggleReactionQuery(NewsId, "Like"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        people.Verify(
            x => x.ToggleNewsReactionAsync(
                NewsId,
                It.Is<object>(b => b.ToString()!.Contains("Like")),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ListComments_delegates()
    {
        var people = new Mock<IPeopleDownstreamClient>();
        people.Setup(x => x.GetNewsContributionsAsync(NewsId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.WithSuccess(JsonDocument.Parse("""[]""").RootElement));

        var handler = new MobileFeedActions.ListCommentsHandler(people.Object);

        var result = await handler.Handle(new MobileFeedActions.ListCommentsQuery(NewsId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        people.Verify(x => x.GetNewsContributionsAsync(NewsId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreateComment_rejects_empty(string body)
    {
        var people = new Mock<IPeopleDownstreamClient>();
        var handler = new MobileFeedActions.CreateCommentHandler(people.Object);

        var result = await handler.Handle(
            new MobileFeedActions.CreateCommentQuery(NewsId, body),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        people.Verify(x => x.CreateNewsContributionAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateComment_forwards_body()
    {
        var people = new Mock<IPeopleDownstreamClient>();
        people.Setup(x => x.CreateNewsContributionAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.WithSuccess(JsonDocument.Parse("""{"id":"c1"}""").RootElement));

        var handler = new MobileFeedActions.CreateCommentHandler(people.Object);

        var result = await handler.Handle(
            new MobileFeedActions.CreateCommentQuery(NewsId, "  Nice update!  "),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        people.Verify(
            x => x.CreateNewsContributionAsync(
                It.Is<object>(b =>
                    b.ToString()!.Contains("Nice update!") && b.ToString()!.Contains(NewsId.ToString())),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
