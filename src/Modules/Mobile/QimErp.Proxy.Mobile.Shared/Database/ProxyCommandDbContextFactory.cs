using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using QimErp.Shared.Common.Services.MultiTenancy;

namespace QimErp.Proxy.Mobile.Shared.Database;

public sealed class ProxyCommandDbContextFactory : IDesignTimeDbContextFactory<ProxyCommandDbContext>
{
    public ProxyCommandDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__ProxyCommands")
            ?? "Host=localhost;Database=qimerp_proxy_commands;Username=qimerp;Password=qimerp";
        var options = new DbContextOptionsBuilder<ProxyCommandDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new ProxyCommandDbContext(options, new DesignTimeTenantContext());
    }

    private sealed class DesignTimeTenantContext : ITenantContext
    {
        public string? TenantId => null;
        public void SetTenant(string? id) { }
    }
}
