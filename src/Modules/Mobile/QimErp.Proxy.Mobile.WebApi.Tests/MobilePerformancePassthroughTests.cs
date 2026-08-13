using System.Text.Json;
using FluentAssertions;
using Moq;
using QFace.Sdk.Extensions;
using QimErp.Proxy.Mobile.Shared.Clients;
using QimErp.Proxy.Mobile.WebApi.Features.Performance;
using QimErp.Proxy.Mobile.WebApi.Services;

namespace QimErp.Proxy.Mobile.WebApi.Tests;

public class MobilePerformancePassthroughTests
{
    [Fact]
    public async Task EssHome_resolves_employee_id_from_token()
    {
        var performance = new Mock<IPerformanceDownstreamClient>();
        var currentUser = new Mock<ICurrentEmployeeResolver>();
        var employeeId = Guid.NewGuid();
        currentUser.Setup(x => x.ResolveAsync(It.IsAny<CancellationToken>())).ReturnsAsync((Guid?)employeeId);
        performance.Setup(x => x.GetEssHomeAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.WithSuccess(JsonDocument.Parse("""{"goals":[]}""").RootElement));

        var handler = new GetMobilePerformanceEssHome.Handler(performance.Object, currentUser.Object);

        var result = await handler.Handle(new GetMobilePerformanceEssHome.Query(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        performance.Verify(x => x.GetEssHomeAsync(employeeId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task EssHome_fails_without_employee_id()
    {
        var performance = new Mock<IPerformanceDownstreamClient>();
        var currentUser = new Mock<ICurrentEmployeeResolver>();
        currentUser.Setup(x => x.ResolveAsync(It.IsAny<CancellationToken>())).ReturnsAsync((Guid?)null);

        var handler = new GetMobilePerformanceEssHome.Handler(performance.Object, currentUser.Object);

        var result = await handler.Handle(new GetMobilePerformanceEssHome.Query(), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Code.Should().Be("400");
        performance.Verify(x => x.GetEssHomeAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ReviewsPage_forwards_body()
    {
        var performance = new Mock<IPerformanceDownstreamClient>();
        performance.Setup(x => x.GetReviewsPageAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.WithSuccess(JsonDocument.Parse("""{"items":[],"totalCount":0}""").RootElement));

        var handler = new GetMobileReviewsPage.Handler(performance.Object);

        var result = await handler.Handle(
            new GetMobileReviewsPage.Command
            {
                Body = JsonDocument.Parse("""{"pageNumber":1,"pageSize":50,"reviewerEmployeeId":"m1"}""").RootElement
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        performance.Verify(
            x => x.GetReviewsPageAsync(
                It.Is<object>(b => b.ToString()!.Contains("reviewerEmployeeId")),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task TeamReviews_resolves_reviewer_from_token()
    {
        var performance = new Mock<IPerformanceDownstreamClient>();
        var currentUser = new Mock<ICurrentEmployeeResolver>();
        var reviewerId = Guid.NewGuid();
        currentUser.Setup(x => x.ResolveAsync(It.IsAny<CancellationToken>())).ReturnsAsync((Guid?)reviewerId);
        performance.Setup(x => x.GetTeamReviewsAsync(reviewerId, It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.WithSuccess(JsonDocument.Parse("""{"items":[]}""").RootElement));

        var handler = new GetMobileTeamReviews.Handler(performance.Object, currentUser.Object);

        var result = await handler.Handle(
            new GetMobileTeamReviews.Command { Body = JsonDocument.Parse("""{}""").RootElement },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        performance.Verify(
            x => x.GetTeamReviewsAsync(reviewerId, It.IsAny<object>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Review_detail_delegates_with_id()
    {
        var performance = new Mock<IPerformanceDownstreamClient>();
        var id = Guid.NewGuid();
        performance.Setup(x => x.GetReviewAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.WithSuccess(JsonDocument.Parse("""{"id":"r1"}""").RootElement));

        var handler = new GetMobileReview.Handler(performance.Object);

        var result = await handler.Handle(new GetMobileReview.Query { Id = id }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        performance.Verify(x => x.GetReviewAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Goals_resolves_employee_id_from_token()
    {
        var performance = new Mock<IPerformanceDownstreamClient>();
        var currentUser = new Mock<ICurrentEmployeeResolver>();
        var employeeId = Guid.NewGuid();
        currentUser.Setup(x => x.ResolveAsync(It.IsAny<CancellationToken>())).ReturnsAsync((Guid?)employeeId);
        performance.Setup(x => x.GetGoalsAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.WithSuccess(JsonDocument.Parse("""[]""").RootElement));

        var handler = new GetMobileGoals.Handler(performance.Object, currentUser.Object);

        var result = await handler.Handle(new GetMobileGoals.Query(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        performance.Verify(x => x.GetGoalsAsync(employeeId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Item_details_delegate_with_ids()
    {
        var performance = new Mock<IPerformanceDownstreamClient>();
        var planId = Guid.NewGuid();
        var checkInId = Guid.NewGuid();
        var feedbackId = Guid.NewGuid();
        var devPlanId = Guid.NewGuid();

        performance.Setup(x => x.GetAppraisalPlanAsync(planId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.WithSuccess(JsonDocument.Parse("""{}""").RootElement));
        performance.Setup(x => x.GetCheckInAsync(checkInId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.WithSuccess(JsonDocument.Parse("""{}""").RootElement));
        performance.Setup(x => x.GetFeedback360Async(feedbackId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.WithSuccess(JsonDocument.Parse("""{}""").RootElement));
        performance.Setup(x => x.GetDevelopmentPlanAsync(devPlanId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.WithSuccess(JsonDocument.Parse("""{}""").RootElement));

        var appraisalHandler = new GetMobileAppraisalPlan.Handler(performance.Object);
        var checkInHandler = new GetMobileCheckIn.Handler(performance.Object);
        var feedbackHandler = new GetMobileFeedback360.Handler(performance.Object);
        var devPlanHandler = new GetMobileDevelopmentPlan.Handler(performance.Object);

        (await appraisalHandler.Handle(new GetMobileAppraisalPlan.Query { Id = planId }, CancellationToken.None))
            .IsSuccess.Should().BeTrue();
        (await checkInHandler.Handle(new GetMobileCheckIn.Query { Id = checkInId }, CancellationToken.None))
            .IsSuccess.Should().BeTrue();
        (await feedbackHandler.Handle(new GetMobileFeedback360.Query { Id = feedbackId }, CancellationToken.None))
            .IsSuccess.Should().BeTrue();
        (await devPlanHandler.Handle(new GetMobileDevelopmentPlan.Query { Id = devPlanId }, CancellationToken.None))
            .IsSuccess.Should().BeTrue();

        performance.Verify(x => x.GetAppraisalPlanAsync(planId, It.IsAny<CancellationToken>()), Times.Once);
        performance.Verify(x => x.GetCheckInAsync(checkInId, It.IsAny<CancellationToken>()), Times.Once);
        performance.Verify(x => x.GetFeedback360Async(feedbackId, It.IsAny<CancellationToken>()), Times.Once);
        performance.Verify(x => x.GetDevelopmentPlanAsync(devPlanId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
