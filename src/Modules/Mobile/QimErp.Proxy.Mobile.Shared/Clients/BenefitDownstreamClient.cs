namespace QimErp.Proxy.Mobile.Shared.Clients;

public interface IBenefitDownstreamClient
{
    Task<Result<JsonElement>> GetEmployeeEnrollmentsAsync(Guid employeeId, bool activeOnly, CancellationToken cancellationToken = default);
}

public sealed class BenefitDownstreamClient(
    IHttpClientFactory httpClientFactory,
    IHttpContextAccessor httpContextAccessor,
    ILogger<BenefitDownstreamClient> logger)
    : DownstreamHttpClientBase(httpClientFactory, httpContextAccessor, logger), IBenefitDownstreamClient
{
    protected override string ClientName => DownstreamClientNames.Benefit;

    public Task<Result<JsonElement>> GetEmployeeEnrollmentsAsync(Guid employeeId, bool activeOnly, CancellationToken cancellationToken = default)
        => GetRawAsync(
            $"{string.Format(MobileApiConstants.Downstream.BenefitEmployeeEnrollments, employeeId)}?activeOnly={activeOnly}",
            cancellationToken);
}
