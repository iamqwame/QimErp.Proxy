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
    public async Task Surveys_available_forwards_body_to_surveys_service()
    {
        var surveys = new Mock<ISurveysDownstreamClient>();
        surveys.Setup(x => x.GetAvailablePageAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.WithSuccess(JsonDocument.Parse("""{"items":[]}""").RootElement));

        var handler = new GetMobileAvailableSurveys.Handler(surveys.Object);

        var result = await handler.Handle(
            new GetMobileAvailableSurveys.Command { Body = JsonDocument.Parse("""{"pageNumber":1}""").RootElement },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        surveys.Verify(x => x.GetAvailablePageAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Surveys_start_forwards_body_to_surveys_service()
    {
        var surveys = new Mock<ISurveysDownstreamClient>();
        surveys.Setup(x => x.StartResponseAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.WithSuccess(JsonDocument.Parse("""{"id":"00000000-0000-0000-0000-000000000001"}""").RootElement));

        var handler = new StartMobileSurveyResponse.Handler(surveys.Object);

        var result = await handler.Handle(
            new StartMobileSurveyResponse.Command
            {
                Body = JsonDocument.Parse("""{"surveyId":"00000000-0000-0000-0000-000000000099"}""").RootElement
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        surveys.Verify(x => x.StartResponseAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Surveys_save_progress_forwards_to_surveys_service()
    {
        var responseId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var surveys = new Mock<ISurveysDownstreamClient>();
        surveys.Setup(x => x.SaveProgressAsync(responseId, It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.WithSuccess(JsonDocument.Parse("""{"id":"00000000-0000-0000-0000-000000000001"}""").RootElement));

        var handler = new SaveMobileSurveyProgress.Handler(surveys.Object);

        var result = await handler.Handle(
            new SaveMobileSurveyProgress.Command
            {
                ResponseId = responseId,
                Body = JsonDocument.Parse("""{"answers":[]}""").RootElement
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        surveys.Verify(x => x.SaveProgressAsync(responseId, It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Surveys_submit_forwards_to_surveys_service()
    {
        var responseId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var surveys = new Mock<ISurveysDownstreamClient>();
        surveys.Setup(x => x.SubmitResponseAsync(responseId, It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.WithSuccess(JsonDocument.Parse("""{"id":"00000000-0000-0000-0000-000000000001"}""").RootElement));

        var handler = new SubmitMobileSurveyResponse.Handler(surveys.Object);

        var result = await handler.Handle(
            new SubmitMobileSurveyResponse.Command
            {
                ResponseId = responseId,
                Body = JsonDocument.Parse("""{}""").RootElement
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        surveys.Verify(x => x.SubmitResponseAsync(responseId, It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
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
