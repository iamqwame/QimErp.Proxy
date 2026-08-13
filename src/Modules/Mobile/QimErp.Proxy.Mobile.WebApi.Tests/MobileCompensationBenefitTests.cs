using System.Text.Json;
using FluentAssertions;
using Moq;
using QFace.Sdk.Extensions;
using QimErp.Proxy.Mobile.Shared.Clients;
using QimErp.Proxy.Mobile.WebApi.Features.Benefits;
using QimErp.Proxy.Mobile.WebApi.Features.Compensation;
using QimErp.Proxy.Mobile.WebApi.Services;

namespace QimErp.Proxy.Mobile.WebApi.Tests;

public class MobileCompensationBenefitTests
{
    [Fact]
    public async Task Compensation_delegates_to_payroll_summary()
    {
        var payroll = new Mock<IPayrollDownstreamClient>();
        payroll.Setup(x => x.GetMySummaryAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.WithSuccess(JsonDocument.Parse(
                """{"currentPeriod":{"netPay":1240.50},"yearToDate":{"netPay":5000}}""").RootElement));

        var handler = new GetMobileCompensation.Handler(payroll.Object);

        var result = await handler.Handle(new GetMobileCompensation.Query(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.GetProperty("currentPeriod").GetProperty("netPay").GetDecimal().Should().Be(1240.50m);
        payroll.Verify(x => x.GetMySummaryAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Benefits_resolves_employee_id_and_queries_active_enrollments()
    {
        var benefit = new Mock<IBenefitDownstreamClient>();
        var currentUser = new Mock<ICurrentEmployeeResolver>();
        var employeeId = Guid.NewGuid();
        currentUser.Setup(x => x.ResolveAsync(It.IsAny<CancellationToken>())).ReturnsAsync((Guid?)employeeId);
        benefit.Setup(x => x.GetEmployeeEnrollmentsAsync(employeeId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.WithSuccess(JsonDocument.Parse(
                """[{"benefitPlanName":"Health","status":"Active"}]""").RootElement));

        var handler = new GetMobileBenefits.Handler(benefit.Object, currentUser.Object);

        var result = await handler.Handle(new GetMobileBenefits.Query(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.GetArrayLength().Should().Be(1);
        benefit.Verify(
            x => x.GetEmployeeEnrollmentsAsync(employeeId, true, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
