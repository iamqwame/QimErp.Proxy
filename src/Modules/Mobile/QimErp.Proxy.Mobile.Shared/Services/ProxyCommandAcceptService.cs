using Microsoft.EntityFrameworkCore;
using QimErp.Proxy.Mobile.Shared.Database;
using QimErp.Proxy.Mobile.Shared.Entities;

namespace QimErp.Proxy.Mobile.Shared.Services;

public sealed record ProxyCommandAcceptResult(ProxyCommand Command, bool WasAlreadyAccepted);

/// <summary>
/// DATA-10: Accept is safe to run twice for the same (TenantId, IdempotencyKey).
/// DATA-24: blank TenantId fails closed before any command store query.
/// </summary>
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

public sealed class ProxyCommandAcceptService(ProxyCommandDbContext db) : IProxyCommandAcceptService
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
        // DATA-24: fail closed before tenant-scoped command lookup — never probe without TenantId.
        EnsureTenantId(tenantId);
        if (string.IsNullOrWhiteSpace(idempotencyKey))
            throw new ArgumentException("IdempotencyKey is required.", nameof(idempotencyKey));

        var key = idempotencyKey.Trim();
        var existing = await db.ProxyCommands
            .FirstOrDefaultAsync(c => c.TenantId == tenantId && c.IdempotencyKey == key, cancellationToken);
        if (existing is not null)
            return new ProxyCommandAcceptResult(existing, WasAlreadyAccepted: true);

        var command = ProxyCommand.Accept(
            tenantId, requestedByUserId, targetService, route, method, payloadCiphertext, key);
        db.ProxyCommands.Add(command);
        await db.SaveChangesAsync(cancellationToken);
        return new ProxyCommandAcceptResult(command, WasAlreadyAccepted: false);
    }

    // DATA-24: blank TenantId must never reach ProxyCommands queries.
    private static void EnsureTenantId(string tenantId)
    {
        if (string.IsNullOrWhiteSpace(tenantId))
            throw new ArgumentException("TenantId is required.", nameof(tenantId));
    }
}
