using Microsoft.EntityFrameworkCore;
using QimErp.Proxy.Mobile.Shared.Database;
using QimErp.Proxy.Mobile.Shared.Entities;

namespace QimErp.Proxy.Mobile.Shared.Services;

public sealed record ProxyCommandAcceptResult(ProxyCommand Command, bool WasAlreadyAccepted);

public interface IProxyCommandAcceptService
{
    Task<ProxyCommandAcceptResult> AcceptAsync(
        string tenantId,
        string requestedByUserId,
        string targetService,
        string route,
        string method,
        string payloadCiphertext,
        string idempotencyKey,
        CancellationToken cancellationToken = default);
}

public sealed class ProxyCommandAcceptService(
    ProxyCommandDbContext db,
    IProxyCommandPayloadProtector payloadProtector) : IProxyCommandAcceptService
{
    public async Task<ProxyCommandAcceptResult> AcceptAsync(
        string tenantId,
        string requestedByUserId,
        string targetService,
        string route,
        string method,
        string payloadCiphertext,
        string idempotencyKey,
        CancellationToken cancellationToken = default)
    {
        // DATA-24: fail closed before tenant-scoped command lookup.
        EnsureTenantId(tenantId);
        if (string.IsNullOrWhiteSpace(idempotencyKey))
            throw new ArgumentException("IdempotencyKey is required.", nameof(idempotencyKey));

        var key = idempotencyKey.Trim();
        // DATA-10: client retries with the same idempotency key return the existing command.
        var existing = await db.ProxyCommands
            .FirstOrDefaultAsync(c => c.TenantId == tenantId && c.IdempotencyKey == key, cancellationToken);
        if (existing is not null)
            return new ProxyCommandAcceptResult(existing, WasAlreadyAccepted: true);

        var protectedPayload = payloadProtector.Protect(payloadCiphertext);
        var command = ProxyCommand.Accept(
            tenantId, requestedByUserId, targetService, route, method, protectedPayload, key);
        db.ProxyCommands.Add(command);
        try
        {
            await db.SaveChangesAsync(cancellationToken);
            return new ProxyCommandAcceptResult(command, WasAlreadyAccepted: false);
        }
        catch (DbUpdateException)
        {
            db.Entry(command).State = EntityState.Detached;
            var raced = await db.ProxyCommands
                .FirstOrDefaultAsync(c => c.TenantId == tenantId && c.IdempotencyKey == key, cancellationToken);
            if (raced is not null)
                return new ProxyCommandAcceptResult(raced, WasAlreadyAccepted: true);
            throw;
        }
    }

    private static void EnsureTenantId(string tenantId)
    {
        if (string.IsNullOrWhiteSpace(tenantId))
            throw new ArgumentException("TenantId is required.", nameof(tenantId));
    }
}
