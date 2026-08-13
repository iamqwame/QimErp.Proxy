namespace QimErp.Proxy.Mobile.Shared.Clients;

public interface ISurveysDownstreamClient
{
    Task<Result<JsonElement>> GetMyResponsesPageAsync(object body, CancellationToken cancellationToken = default);
}

public sealed class SurveysDownstreamClient(
    IHttpClientFactory httpClientFactory,
    IHttpContextAccessor httpContextAccessor,
    ILogger<SurveysDownstreamClient> logger)
    : DownstreamHttpClientBase(httpClientFactory, httpContextAccessor, logger), ISurveysDownstreamClient
{
    protected override string ClientName => DownstreamClientNames.Surveys;

    public Task<Result<JsonElement>> GetMyResponsesPageAsync(object body, CancellationToken cancellationToken = default)
        => PostRawAsync(MobileApiConstants.Downstream.SurveysMyResponsesPage, body, cancellationToken);
}
