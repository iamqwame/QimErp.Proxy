namespace QimErp.Proxy.Mobile.Shared.Clients;

public interface IWorkflowDownstreamClient
{
    Task<Result<JsonElement>> GetPendingAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> ApproveAsync(object body, CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> RejectAsync(object body, CancellationToken cancellationToken = default);
}

public sealed class WorkflowDownstreamClient(
    IHttpClientFactory httpClientFactory,
    IHttpContextAccessor httpContextAccessor,
    ILogger<WorkflowDownstreamClient> logger)
    : DownstreamHttpClientBase(httpClientFactory, httpContextAccessor, logger), IWorkflowDownstreamClient
{
    protected override string ClientName => DownstreamClientNames.Workflow;

    public Task<Result<JsonElement>> GetPendingAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        => GetRawAsync($"{MobileApiConstants.Downstream.WorkflowPending}?page={page}&pageSize={pageSize}", cancellationToken);

    public Task<Result<JsonElement>> ApproveAsync(object body, CancellationToken cancellationToken = default)
        => PostRawAsync(MobileApiConstants.Downstream.WorkflowApprove, body, cancellationToken);

    public Task<Result<JsonElement>> RejectAsync(object body, CancellationToken cancellationToken = default)
        => PostRawAsync(MobileApiConstants.Downstream.WorkflowReject, body, cancellationToken);
}
