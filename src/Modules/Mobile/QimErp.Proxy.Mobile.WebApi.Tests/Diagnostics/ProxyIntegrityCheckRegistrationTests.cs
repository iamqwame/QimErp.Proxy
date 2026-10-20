using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using QimErp.Proxy.Mobile.Shared.Database;
using QimErp.Proxy.Mobile.WebApi.Tests.Integration;
using QimErp.Shared.Common.Diagnostics.IntegrityChecks;

namespace QimErp.Proxy.Mobile.WebApi.Tests.Diagnostics;

public sealed class ProxyIntegrityCheckRegistrationTests(ProxyInMemoryWebAppFactory factory)
    : IClassFixture<ProxyInMemoryWebAppFactory>
{
    [Fact(DisplayName = "Proxy Mobile WebApi registers DATA-32 outbox drift backstop (integrity runner + OutboxBackstopIntegrityCheck)")]
    public void Host_registers_integrity_check_runner_and_outbox_backstop_check()
    {
        factory.Services.GetServices<IHostedService>()
            .OfType<IntegrityCheckRunnerBackgroundService>()
            .Should()
            .ContainSingle();

        using var scope = factory.Services.CreateScope();
        var checks = scope.ServiceProvider.GetServices<IIntegrityCheck>().ToList();
        checks.Should().ContainSingle(c => c is OutboxBackstopIntegrityCheck<ProxyCommandDbContext>);

        var options = factory.Services.GetRequiredService<IOptions<IntegrityCheckRunnerOptions>>().Value;
        options.PollingIntervalSeconds.Should().Be(86_400);
    }
}
