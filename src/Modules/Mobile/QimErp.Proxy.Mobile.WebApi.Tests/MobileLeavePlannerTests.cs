using System.Text.Json;
using FluentAssertions;
using Moq;
using QFace.Sdk.Extensions;
using QimErp.Proxy.Mobile.Shared.Clients;
using QimErp.Proxy.Mobile.WebApi.Features.TimeOff;

namespace QimErp.Proxy.Mobile.WebApi.Tests;

public class MobileLeavePlannerTests
{
    [Fact]
    public async Task Planner_defaults_scope_to_mine()
    {
        var leave = new Mock<ILeaveDownstreamClient>();
        leave.Setup(x => x.GetPlannerAsync("mine", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.WithSuccess(JsonDocument.Parse("""[]""").RootElement));

        var handler = new GetMobilePlanner.Handler(leave.Object);

        var result = await handler.Handle(new GetMobilePlanner.Query(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        leave.Verify(x => x.GetPlannerAsync("mine", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Holidays_forwards_year()
    {
        var leave = new Mock<ILeaveDownstreamClient>();
        leave.Setup(x => x.GetHolidaysAsync(2026, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.WithSuccess(JsonDocument.Parse("""[]""").RootElement));

        var handler = new GetMobileHolidays.Handler(leave.Object);

        var result = await handler.Handle(
            new GetMobileHolidays.Query { Year = 2026 },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        leave.Verify(x => x.GetHolidaysAsync(2026, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Travel_my_forwards_body()
    {
        var leave = new Mock<ILeaveDownstreamClient>();
        leave.Setup(x => x.GetTravelPermissionsAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.WithSuccess(JsonDocument.Parse("""{"items":[]}""").RootElement));

        var handler = new GetMobileTravelPermissions.Handler(leave.Object);

        var result = await handler.Handle(
            new GetMobileTravelPermissions.Command { Body = JsonDocument.Parse("""{}""").RootElement },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        leave.Verify(x => x.GetTravelPermissionsAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
