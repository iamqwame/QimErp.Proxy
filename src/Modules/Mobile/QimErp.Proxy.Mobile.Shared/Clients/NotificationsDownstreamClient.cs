namespace QimErp.Proxy.Mobile.Shared.Clients;

public interface INotificationsDownstreamClient
{
    Task<Result<JsonElement>> GetHistoryPageAsync(object body, CancellationToken cancellationToken = default);
}

public sealed class NotificationsDownstreamClient(
    IHttpClientFactory httpClientFactory,
    IHttpContextAccessor httpContextAccessor,
    ILogger<NotificationsDownstreamClient> logger)
    : DownstreamHttpClientBase(httpClientFactory, httpContextAccessor, logger), INotificationsDownstreamClient
{
    protected override string ClientName => DownstreamClientNames.Notifications;

    public Task<Result<JsonElement>> GetHistoryPageAsync(object body, CancellationToken cancellationToken = default)
        => PostRawAsync(MobileApiConstants.Downstream.NotificationsHistoryPage, body, cancellationToken);
}
