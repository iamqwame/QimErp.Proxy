using QimErp.Proxy.Mobile.Shared.Contracts;
using QimErp.Shared.Common.Extensions;

namespace QimErp.Proxy.Mobile.Shared.Clients;

public interface IIamDownstreamClient
{
    Task<Result<JsonElement>> LoginAsync(object body, CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> RefreshAsync(object body, CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> GetMeAsync(CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> GetTenantConfigAsync(string domain, CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> CompleteTwoFactorLoginAsync(object body, CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> GetSessionsAsync(CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> RevokeSessionAsync(object body, CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> RevokeAllSessionsAsync(CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> ChangePasswordAsync(object body, CancellationToken cancellationToken = default);
    Task<Result<PaginatedList<AuditActivityResponse>>> GetMyActivitiesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}

public sealed class IamDownstreamClient(
    IHttpClientFactory httpClientFactory,
    IHttpContextAccessor httpContextAccessor,
    ILogger<IamDownstreamClient> logger)
    : DownstreamHttpClientBase(httpClientFactory, httpContextAccessor, logger), IIamDownstreamClient
{
    protected override string ClientName => DownstreamClientNames.Iam;

    public Task<Result<JsonElement>> LoginAsync(object body, CancellationToken cancellationToken = default)
        => PostRawAsync(MobileApiConstants.Downstream.IamLogin, body, cancellationToken);

    public Task<Result<JsonElement>> RefreshAsync(object body, CancellationToken cancellationToken = default)
        => PostRawAsync(MobileApiConstants.Downstream.IamRefresh, body, cancellationToken);

    public Task<Result<JsonElement>> GetMeAsync(CancellationToken cancellationToken = default)
        => GetRawAsync(MobileApiConstants.Downstream.IamMe, cancellationToken);

    public Task<Result<JsonElement>> GetTenantConfigAsync(string domain, CancellationToken cancellationToken = default)
        => GetRawAsync(string.Format(MobileApiConstants.Downstream.IamTenantConfig, domain), cancellationToken);

    public Task<Result<JsonElement>> CompleteTwoFactorLoginAsync(object body, CancellationToken cancellationToken = default)
        => PostRawAsync(MobileApiConstants.Downstream.IamCompleteTwoFactor, body, cancellationToken);

    public Task<Result<JsonElement>> GetSessionsAsync(CancellationToken cancellationToken = default)
        => GetRawAsync(MobileApiConstants.Downstream.IamSessions, cancellationToken);

    public Task<Result<JsonElement>> RevokeSessionAsync(object body, CancellationToken cancellationToken = default)
        => PostRawAsync(MobileApiConstants.Downstream.IamSessionsRevoke, body, cancellationToken);

    public Task<Result<JsonElement>> RevokeAllSessionsAsync(CancellationToken cancellationToken = default)
        => PostRawAsync(MobileApiConstants.Downstream.IamSessionsRevokeAll, null, cancellationToken);

    public Task<Result<JsonElement>> ChangePasswordAsync(object body, CancellationToken cancellationToken = default)
        => PostRawAsync(MobileApiConstants.Downstream.IamChangePassword, body, cancellationToken);

    public Task<Result<PaginatedList<AuditActivityResponse>>> GetMyActivitiesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        => GetAsync<PaginatedList<AuditActivityResponse>>(
            $"{MobileApiConstants.Downstream.IamMyActivities}?pageNumber={pageNumber}&pageSize={pageSize}",
            cancellationToken);
}
