using QimErp.Shared.Common.Entities;
using QimErp.Shared.Common.Security;

namespace QimErp.Proxy.Mobile.Shared.Entities;

public sealed class ProxyCommand : GuidAuditableEntity
{
    public string TargetService { get; private set; } = string.Empty;
    public string Route { get; private set; } = string.Empty;
    public string Method { get; private set; } = string.Empty;

    // DATA-25: command body is Sensitive — AES-GCM ciphertext only (never plaintext at rest).
    [DataClassification(DataClassification.Sensitive)]
    public string PayloadCiphertext { get; private set; } = string.Empty;

    [DataClassification(DataClassification.Internal)]
    public string RequestedByUserId { get; private set; } = string.Empty;
    public string IdempotencyKey { get; private set; } = string.Empty;
    public string Status { get; private set; } = Pending;
    public DateTime AcceptedAtUtc { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }
    public string? FailureCode { get; private set; }

    public const string Pending = "pending";
    public const string Completed = "completed";
    public const string Failed = "failed";

    private ProxyCommand() { }

    public static ProxyCommand Accept(
        string tenantId,
        string requestedByUserId,
        string targetService,
        string route,
        string method,
        string payloadCiphertext,
        string idempotencyKey)
    {
        if (string.IsNullOrWhiteSpace(idempotencyKey))
            throw new ArgumentException("IdempotencyKey is required.", nameof(idempotencyKey));

        var command = new ProxyCommand
        {
            Id = CreateId(),
            TenantId = tenantId,
            RequestedByUserId = requestedByUserId,
            TargetService = targetService,
            Route = route,
            Method = method,
            PayloadCiphertext = payloadCiphertext,
            IdempotencyKey = idempotencyKey.Trim(),
            AcceptedAtUtc = DateTime.UtcNow
        };
        command.AsActive();
        return command;
    }

    public void Complete()
    {
        Status = Completed;
        CompletedAtUtc = DateTime.UtcNow;
        FailureCode = null;
    }

    public void Fail(string failureCode)
    {
        Status = Failed;
        CompletedAtUtc = DateTime.UtcNow;
        FailureCode = failureCode;
    }
}
