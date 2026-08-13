using System.Text.Json;
using FluentAssertions;
using Moq;
using QFace.Sdk.Extensions;
using QimErp.Proxy.Mobile.Shared.Clients;
using QimErp.Proxy.Mobile.WebApi.Features.Home;
using QimErp.Proxy.Mobile.WebApi.Services;

namespace QimErp.Proxy.Mobile.WebApi.Tests;

public class GetMobileHomeDegradeTests
{
    [Fact]
    public async Task Handle_marks_degraded_sections_when_downstream_fails()
    {
        var performance = new Mock<IPerformanceDownstreamClient>();
        var leave = new Mock<ILeaveDownstreamClient>();
        var workflow = new Mock<IWorkflowDownstreamClient>();
        var payroll = new Mock<IPayrollDownstreamClient>();
        var currentUser = new Mock<ICurrentEmployeeResolver>();

        currentUser.Setup(x => x.ResolveAsync(It.IsAny<CancellationToken>())).ReturnsAsync((Guid?)Guid.NewGuid());

        performance.Setup(x => x.GetEssHomeAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.WithFailure<JsonElement>(new Error("down", "ess down"), code: "503"));
        leave.Setup(x => x.GetBalanceAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.WithSuccess(JsonDocument.Parse("{\"days\":5}").RootElement));
        workflow.Setup(x => x.GetPendingAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.WithFailure<JsonElement>(new Error("down", "wf down"), code: "503"));
        payroll.Setup(x => x.GetMySummaryAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.WithSuccess(JsonDocument.Parse("{\"net\":1}").RootElement));

        var handler = new GetMobileHome.Handler(
            performance.Object,
            leave.Object,
            workflow.Object,
            payroll.Object,
            currentUser.Object);

        var result = await handler.Handle(new GetMobileHome.Query(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.DegradedSections.Should().Contain(["essHome", "pendingApprovals"]);
        result.Data.LeaveBalance.Should().NotBeNull();
        result.Data.PayrollSummary.Should().NotBeNull();
    }
}
