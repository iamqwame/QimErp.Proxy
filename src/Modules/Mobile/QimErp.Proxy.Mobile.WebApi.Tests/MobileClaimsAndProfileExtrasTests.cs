using System.Text.Json;
using FluentAssertions;
using Moq;
using QFace.Sdk.Extensions;
using QimErp.Proxy.Mobile.Shared.Clients;
using QimErp.Proxy.Mobile.WebApi.Features.Payroll;
using QimErp.Proxy.Mobile.WebApi.Features.Profile;
using QimErp.Proxy.Mobile.WebApi.Services;

namespace QimErp.Proxy.Mobile.WebApi.Tests;

public class MobileClaimsAndProfileExtrasTests
{
    [Fact]
    public async Task GetClaims_delegates()
    {
        var payroll = new Mock<IPayrollDownstreamClient>();
        payroll.Setup(x => x.GetMyClaimsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.WithSuccess(JsonDocument.Parse("""[]""").RootElement));

        var handler = new GetMobileClaims.Handler(payroll.Object);

        var result = await handler.Handle(new GetMobileClaims.Query(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        payroll.Verify(x => x.GetMyClaimsAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RequestClaim_forwards_body()
    {
        var payroll = new Mock<IPayrollDownstreamClient>();
        payroll.Setup(x => x.RequestMyClaimAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.WithSuccess(JsonDocument.Parse("""{"id":"c1"}""").RootElement));

        var handler = new RequestMobileClaim.Handler(payroll.Object);

        var result = await handler.Handle(
            new RequestMobileClaim.Command
            {
                Body = JsonDocument.Parse("""{"requestedAmount":250,"category":"General"}""").RootElement
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        payroll.Verify(
            x => x.RequestMyClaimAsync(
                It.Is<object>(b => b.ToString()!.Contains("250")),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ProfileEducation_resolves_employee_id()
    {
        var people = new Mock<IPeopleDownstreamClient>();
        var currentUser = new Mock<ICurrentEmployeeResolver>();
        var employeeId = Guid.NewGuid();
        currentUser.Setup(x => x.ResolveAsync(It.IsAny<CancellationToken>())).ReturnsAsync((Guid?)employeeId);
        people.Setup(x => x.GetQualificationsAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.WithSuccess(JsonDocument.Parse("""[]""").RootElement));

        var handler = new GetMobileProfileEducation.Handler(people.Object, currentUser.Object);

        var result = await handler.Handle(new GetMobileProfileEducation.Query(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        people.Verify(x => x.GetQualificationsAsync(employeeId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ProfileDocuments_resolves_employee_id()
    {
        var people = new Mock<IPeopleDownstreamClient>();
        var currentUser = new Mock<ICurrentEmployeeResolver>();
        var employeeId = Guid.NewGuid();
        currentUser.Setup(x => x.ResolveAsync(It.IsAny<CancellationToken>())).ReturnsAsync((Guid?)employeeId);
        people.Setup(x => x.GetDocumentsAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.WithSuccess(JsonDocument.Parse("""[]""").RootElement));

        var handler = new GetMobileProfileDocuments.Handler(people.Object, currentUser.Object);

        var result = await handler.Handle(new GetMobileProfileDocuments.Query(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        people.Verify(x => x.GetDocumentsAsync(employeeId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
