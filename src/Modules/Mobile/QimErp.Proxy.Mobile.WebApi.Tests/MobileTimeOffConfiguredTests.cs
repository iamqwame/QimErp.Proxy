using System.Text.Json;
using FluentAssertions;
using Moq;
using QFace.Sdk.Extensions;
using QimErp.Proxy.Mobile.Shared.Clients;
using QimErp.Proxy.Mobile.WebApi.Features.TimeOff;

namespace QimErp.Proxy.Mobile.WebApi.Tests;

public class MobileTimeOffConfiguredTests
{
    [Fact]
    public async Task GetConfigured_delegates_to_leave()
    {
        var leave = new Mock<ILeaveDownstreamClient>();
        var payload = JsonDocument.Parse(
            """{"leaveTypes":[{"id":"t1","name":"Annual","isUnpaid":false}]}""").RootElement;
        leave.Setup(x => x.GetConfiguredAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.WithSuccess(payload));

        var handler = new GetMobileTimeOffConfigured.Handler(leave.Object);

        var result = await handler.Handle(new GetMobileTimeOffConfigured.Query(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.GetProperty("leaveTypes").GetArrayLength().Should().Be(1);
        leave.Verify(x => x.GetConfiguredAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetTypes_delegates_to_leave()
    {
        var leave = new Mock<ILeaveDownstreamClient>();
        var payload = JsonDocument.Parse(
            """[{"id":"t1","name":"Annual"},{"id":"t2","name":"Sick"}]""").RootElement;
        leave.Setup(x => x.GetLeaveTypesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.WithSuccess(payload));

        var handler = new GetMobileTimeOffTypes.Handler(leave.Object);

        var result = await handler.Handle(new GetMobileTimeOffTypes.Query(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.GetArrayLength().Should().Be(2);
        leave.Verify(x => x.GetLeaveTypesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Calculate_builds_query_and_delegates_to_leave()
    {
        var leave = new Mock<ILeaveDownstreamClient>();
        var payload = JsonDocument.Parse("""{"availableDays":18}""").RootElement;
        var leaveTypeId = Guid.NewGuid();
        leave.Setup(x => x.CalculateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.WithSuccess(payload));

        var handler = new CalculateMobileTimeOff.Handler(leave.Object);

        var result = await handler.Handle(
            new CalculateMobileTimeOff.Query
            {
                LeaveTypeId = leaveTypeId,
                AsOfDate = new DateTime(2026, 8, 10)
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.GetProperty("availableDays").GetInt32().Should().Be(18);
        leave.Verify(
            x => x.CalculateAsync(
                It.Is<string>(q =>
                    q.Contains(leaveTypeId.ToString()) && q.Contains("2026-08-10")),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
