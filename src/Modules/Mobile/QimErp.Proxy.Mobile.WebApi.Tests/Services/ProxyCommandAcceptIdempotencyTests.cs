using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using QimErp.Proxy.Mobile.Shared.Database;
using QimErp.Proxy.Mobile.Shared.Services;
using QimErp.Proxy.Mobile.WebApi.Tests.Integration;
using QimErp.Shared.Common.Services.MultiTenancy;

namespace QimErp.Proxy.Mobile.WebApi.Tests.Services;

public sealed class ProxyCommandAcceptIdempotencyTests : IDisposable
{
    private const string TenantId = "tenant-proxy-idem";
    private readonly TenantContext _tenantContext = new();
    private readonly DbContextOptions<ProxyCommandDbContext> _options;
    private readonly IProxyCommandPayloadProtector _protector;

    public ProxyCommandAcceptIdempotencyTests()
    {
        _tenantContext.SetTenant(TenantId);
        _options = new DbContextOptionsBuilder<ProxyCommandDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ReplaceService<IModelCustomizer, ProxyJsonColumnModelCustomizer>()
            .Options;

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ProxyCommandProtection:Key"] = Convert.ToBase64String(new byte[32]),
            })
            .Build();
        _protector = new ProxyCommandPayloadProtector(configuration);

        using var db = CreateDb();
        db.Database.EnsureCreated();
    }

    [Fact]
    public async Task AcceptAsync_twice_same_idempotency_key_returns_existing()
    {
        await using var db = CreateDb();
        var sut = new ProxyCommandAcceptService(db, _protector);

        var first = await sut.AcceptAsync(
            TenantId, "user-1", "people", "/api/x", "POST", "payload-a", "key-1");
        var second = await sut.AcceptAsync(
            TenantId, "user-1", "people", "/api/x", "POST", "payload-b", "key-1");

        first.WasAlreadyAccepted.Should().BeFalse();
        second.WasAlreadyAccepted.Should().BeTrue();
        second.Command.Id.Should().Be(first.Command.Id);

        var count = await db.ProxyCommands.CountAsync(c => c.TenantId == TenantId && c.IdempotencyKey == "key-1");
        count.Should().Be(1);
    }

    [Fact]
    public async Task AcceptAsync_persists_protected_payload_not_plaintext()
    {
        await using var db = CreateDb();
        var sut = new ProxyCommandAcceptService(db, _protector);

        const string plaintext = "sensitive-command-body";
        var result = await sut.AcceptAsync(
            TenantId, "user-1", "people", "/api/x", "POST", plaintext, "key-protect");

        result.Command.PayloadCiphertext.Should().NotBe(plaintext);
        _protector.Unprotect(result.Command.PayloadCiphertext).Should().Be(plaintext);
    }

    public void Dispose()
    {
        using var db = CreateDb();
        db.Database.EnsureDeleted();
    }

    private ProxyCommandDbContext CreateDb() => new(_options, _tenantContext);
}
