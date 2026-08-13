namespace QimErp.Proxy.Mobile.Shared.Clients;

public interface IPayrollDownstreamClient
{
    Task<Result<JsonElement>> GetPayslipsPageAsync(object body, CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> GetMySummaryAsync(CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> GetPayslipAsync(Guid payslipId, CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> AcknowledgePayslipAsync(Guid payslipId, CancellationToken cancellationToken = default);
    Task<Result<DownstreamFile>> DownloadPayslipPdfAsync(Guid payslipId, CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> GetMyClaimsAsync(CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> RequestMyClaimAsync(object body, CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> RequestMyLoanAsync(object body, CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> RequestMyAdvanceAsync(object body, CancellationToken cancellationToken = default);
}

public sealed class PayrollDownstreamClient(
    IHttpClientFactory httpClientFactory,
    IHttpContextAccessor httpContextAccessor,
    ILogger<PayrollDownstreamClient> logger)
    : DownstreamHttpClientBase(httpClientFactory, httpContextAccessor, logger), IPayrollDownstreamClient
{
    protected override string ClientName => DownstreamClientNames.Payroll;

    public Task<Result<JsonElement>> GetPayslipsPageAsync(object body, CancellationToken cancellationToken = default)
        => PostRawAsync(MobileApiConstants.Downstream.PayrollPayslipsPage, body, cancellationToken);

    public Task<Result<JsonElement>> GetMySummaryAsync(CancellationToken cancellationToken = default)
        => GetRawAsync(MobileApiConstants.Downstream.PayrollSummary, cancellationToken);

    public Task<Result<JsonElement>> GetPayslipAsync(Guid payslipId, CancellationToken cancellationToken = default)
        => GetRawAsync(string.Format(MobileApiConstants.Downstream.PayrollPayslip, payslipId), cancellationToken);

    public Task<Result<JsonElement>> AcknowledgePayslipAsync(Guid payslipId, CancellationToken cancellationToken = default)
        => PostRawAsync(string.Format(MobileApiConstants.Downstream.PayrollPayslipAcknowledge, payslipId), null, cancellationToken);

    public Task<Result<DownstreamFile>> DownloadPayslipPdfAsync(Guid payslipId, CancellationToken cancellationToken = default)
        => GetFileAsync(string.Format(MobileApiConstants.Downstream.PayrollPayslipDownload, payslipId), cancellationToken);

    public Task<Result<JsonElement>> GetMyClaimsAsync(CancellationToken cancellationToken = default)
        => GetRawAsync(MobileApiConstants.Downstream.PayrollMyClaims, cancellationToken);

    public Task<Result<JsonElement>> RequestMyClaimAsync(object body, CancellationToken cancellationToken = default)
        => PostRawAsync(MobileApiConstants.Downstream.PayrollMyClaimsRequest, body, cancellationToken);

    public Task<Result<JsonElement>> RequestMyLoanAsync(object body, CancellationToken cancellationToken = default)
        => PostRawAsync(MobileApiConstants.Downstream.PayrollMyLoanRequest, body, cancellationToken);

    public Task<Result<JsonElement>> RequestMyAdvanceAsync(object body, CancellationToken cancellationToken = default)
        => PostRawAsync(MobileApiConstants.Downstream.PayrollMyAdvanceRequest, body, cancellationToken);
}
