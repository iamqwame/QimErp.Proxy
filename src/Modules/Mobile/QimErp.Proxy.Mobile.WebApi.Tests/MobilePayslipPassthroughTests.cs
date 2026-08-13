using System.Text.Json;
using FluentAssertions;
using Moq;
using QFace.Sdk.Extensions;
using QimErp.Proxy.Mobile.Shared.Clients;
using QimErp.Proxy.Mobile.WebApi.Features.Payslips;

namespace QimErp.Proxy.Mobile.WebApi.Tests;

public class MobilePayslipPassthroughTests
{
    [Fact]
    public async Task GetMobilePayslip_delegates_with_id()
    {
        var payroll = new Mock<IPayrollDownstreamClient>();
        var id = Guid.NewGuid();
        var payload = JsonDocument.Parse(
            """{"payPeriod":"2026-07","netPay":1240.50}""").RootElement;
        payroll.Setup(x => x.GetPayslipAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.WithSuccess(payload));

        var handler = new GetMobilePayslip.Handler(payroll.Object);

        var result = await handler.Handle(
            new GetMobilePayslip.Query { Id = id },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.GetProperty("netPay").GetDecimal().Should().Be(1240.50m);
        payroll.Verify(x => x.GetPayslipAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AcknowledgeMobilePayslip_delegates_with_id()
    {
        var payroll = new Mock<IPayrollDownstreamClient>();
        var id = Guid.NewGuid();
        payroll.Setup(x => x.AcknowledgePayslipAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.WithSuccess(JsonDocument.Parse("""{"acknowledged":true}""").RootElement));

        var handler = new AcknowledgeMobilePayslip.Handler(payroll.Object);

        var result = await handler.Handle(
            new AcknowledgeMobilePayslip.Command { Id = id },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.GetProperty("acknowledged").GetBoolean().Should().BeTrue();
        payroll.Verify(x => x.AcknowledgePayslipAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }
}
