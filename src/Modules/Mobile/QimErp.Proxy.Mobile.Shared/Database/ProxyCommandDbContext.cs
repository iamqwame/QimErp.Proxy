using Microsoft.EntityFrameworkCore;
using QimErp.Proxy.Mobile.Shared.Entities;
using QimErp.Shared.Common.Database;
using QimErp.Shared.Common.Services.MultiTenancy;

namespace QimErp.Proxy.Mobile.Shared.Database;

public sealed class ProxyCommandDbContext(
    DbContextOptions<ProxyCommandDbContext> options,
    ITenantContext tenantContext)
    : ApplicationDbContext<ProxyCommandDbContext>(options, tenantContext)
{
    public DbSet<ProxyCommand> ProxyCommands => Set<ProxyCommand>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<ProxyCommand>(builder =>
        {
            builder.ToTable("ProxyCommands");
            builder.Property(x => x.TargetService).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Route).HasMaxLength(500).IsRequired();
            builder.Property(x => x.Method).HasMaxLength(10).IsRequired();
            builder.Property(x => x.PayloadCiphertext).IsRequired();
            builder.Property(x => x.RequestedByUserId).HasMaxLength(128).IsRequired();
            builder.Property(x => x.Status).HasMaxLength(20).IsRequired();
            builder.Property(x => x.FailureCode).HasMaxLength(100);
            builder.HasIndex(x => new { x.TenantId, x.Status, x.AcceptedAtUtc });
        });
    }
}
