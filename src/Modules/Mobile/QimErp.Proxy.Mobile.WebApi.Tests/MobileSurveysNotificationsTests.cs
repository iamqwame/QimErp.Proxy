using System.Text.Json;
using FluentAssertions;
using Moq;
using QFace.Sdk.Extensions;
using QimErp.Proxy.Mobile.Shared.Clients;
using QimErp.Proxy.Mobile.WebApi.Features.Notifications;
using QimErp.Proxy.Mobile.WebApi.Features.Surveys;

namespace QimErp.Proxy.Mobile.WebApi.Tests;

public class MobileSurveysNotificationsTests
{
    [Fact]
    public async Task Surveys_forwards_body_to_surveys_service()
    {
        var surveys = new Mock<ISurveysDownstreamClient>();
        surveys.Setup(x => x.GetMyResponsesPageAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.WithSuccess(JsonDocument.Parse("""{"items":[]}""").RootElement));

        var handler = new GetMobileSurveys.Handler(surveys.Object);

        var result = await handler.Handle(
            new GetMobileSurveys.Command { Body = JsonDocument.Parse("""{"pageSize":10}""").RootElement },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        surveys.Verify(x => x.GetMyResponsesPageAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Notifications_forwards_body_to_platform()
    {
        var notifications = new Mock<INotificationsDownstreamClient>();
        notifications.Setup(x => x.GetHistoryPageAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.WithSuccess(JsonDocument.Parse("""{"items":[]}""").RootElement));

        var handler = new GetMobileNotifications.Handler(notifications.Object);

        var result = await handler.Handle(
            new GetMobileNotifications.Command { Body = JsonDocument.Parse("""{}""").RootElement },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        notifications.Verify(x => x.GetHistoryPageAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
