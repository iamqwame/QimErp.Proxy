namespace QimErp.Proxy.Mobile.Shared.Clients;

public interface ISurveysDownstreamClient
{
    Task<Result<JsonElement>> GetMyResponsesPageAsync(object body, CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> GetAvailablePageAsync(object body, CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> StartResponseAsync(object body, CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> SaveProgressAsync(Guid responseId, object body, CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> SubmitResponseAsync(Guid responseId, object? body, CancellationToken cancellationToken = default);
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

    public Task<Result<JsonElement>> GetAvailablePageAsync(object body, CancellationToken cancellationToken = default)
        => PostRawAsync(MobileApiConstants.Downstream.SurveysAvailablePage, body, cancellationToken);

    public Task<Result<JsonElement>> StartResponseAsync(object body, CancellationToken cancellationToken = default)
        => PostRawAsync(MobileApiConstants.Downstream.SurveysStartResponse, body, cancellationToken);

    public Task<Result<JsonElement>> SaveProgressAsync(Guid responseId, object body, CancellationToken cancellationToken = default)
        => PutRawAsync(string.Format(MobileApiConstants.Downstream.SurveysSaveProgress, responseId), body, cancellationToken);

    public Task<Result<JsonElement>> SubmitResponseAsync(Guid responseId, object? body, CancellationToken cancellationToken = default)
        => PostRawAsync(string.Format(MobileApiConstants.Downstream.SurveysSubmitResponse, responseId), body ?? new { }, cancellationToken);
}
