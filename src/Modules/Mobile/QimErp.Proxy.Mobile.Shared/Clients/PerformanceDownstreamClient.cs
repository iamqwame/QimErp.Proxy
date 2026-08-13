namespace QimErp.Proxy.Mobile.Shared.Clients;

public interface IPerformanceDownstreamClient
{
    Task<Result<JsonElement>> GetEssHomeAsync(Guid employeeId, CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> GetReviewsPageAsync(object body, CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> GetTeamReviewsAsync(Guid reviewerEmployeeId, object body, CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> GetReviewAsync(Guid reviewId, CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> GetReviewSummaryAsync(Guid reviewId, CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> GetGoalsAsync(Guid employeeId, CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> GetAppraisalPlanAsync(Guid planId, CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> GetCheckInAsync(Guid checkInId, CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> CreateCheckInAsync(object body, CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> GetFeedback360Async(Guid feedbackId, CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> GetDevelopmentPlanAsync(Guid planId, CancellationToken cancellationToken = default);
}

public sealed class PerformanceDownstreamClient(
    IHttpClientFactory httpClientFactory,
    IHttpContextAccessor httpContextAccessor,
    ILogger<PerformanceDownstreamClient> logger)
    : DownstreamHttpClientBase(httpClientFactory, httpContextAccessor, logger), IPerformanceDownstreamClient
{
    protected override string ClientName => DownstreamClientNames.Performance;

    public Task<Result<JsonElement>> GetEssHomeAsync(Guid employeeId, CancellationToken cancellationToken = default)
        => GetRawAsync(string.Format(MobileApiConstants.Downstream.PerformanceEssHome, employeeId), cancellationToken);

    public Task<Result<JsonElement>> GetReviewsPageAsync(object body, CancellationToken cancellationToken = default)
        => PostRawAsync(MobileApiConstants.Downstream.PerformanceReviewsPage, body, cancellationToken);

    public Task<Result<JsonElement>> GetTeamReviewsAsync(Guid reviewerEmployeeId, object body, CancellationToken cancellationToken = default)
        => PostRawAsync(MobileApiConstants.Downstream.PerformanceReviewsPage, new
        {
            pageNumber = 1,
            pageSize = 100,
            reviewerEmployeeId = reviewerEmployeeId.ToString(),
            body
        }, cancellationToken);

    public Task<Result<JsonElement>> GetReviewAsync(Guid reviewId, CancellationToken cancellationToken = default)
        => GetRawAsync(string.Format(MobileApiConstants.Downstream.PerformanceReview, reviewId), cancellationToken);

    public Task<Result<JsonElement>> GetReviewSummaryAsync(Guid reviewId, CancellationToken cancellationToken = default)
        => GetRawAsync(string.Format(MobileApiConstants.Downstream.PerformanceReviewSummary, reviewId), cancellationToken);

    public Task<Result<JsonElement>> GetGoalsAsync(Guid employeeId, CancellationToken cancellationToken = default)
        => GetRawAsync(string.Format(MobileApiConstants.Downstream.PerformanceGoalsForEmployee, employeeId), cancellationToken);

    public Task<Result<JsonElement>> GetAppraisalPlanAsync(Guid planId, CancellationToken cancellationToken = default)
        => GetRawAsync(string.Format(MobileApiConstants.Downstream.PerformanceAppraisalPlan, planId), cancellationToken);

    public Task<Result<JsonElement>> GetCheckInAsync(Guid checkInId, CancellationToken cancellationToken = default)
        => GetRawAsync(string.Format(MobileApiConstants.Downstream.PerformanceCheckIn, checkInId), cancellationToken);

    public Task<Result<JsonElement>> CreateCheckInAsync(object body, CancellationToken cancellationToken = default)
        => PostRawAsync(MobileApiConstants.Downstream.PerformanceCheckIns, body, cancellationToken);

    public Task<Result<JsonElement>> GetFeedback360Async(Guid feedbackId, CancellationToken cancellationToken = default)
        => GetRawAsync(string.Format(MobileApiConstants.Downstream.PerformanceFeedback360, feedbackId), cancellationToken);

    public Task<Result<JsonElement>> GetDevelopmentPlanAsync(Guid planId, CancellationToken cancellationToken = default)
        => GetRawAsync(string.Format(MobileApiConstants.Downstream.PerformanceDevelopmentPlan, planId), cancellationToken);
}
