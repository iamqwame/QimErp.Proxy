namespace QimErp.Proxy.Mobile.WebApi.Features.Payslips;

public static class DownloadMobilePayslipPdf
{
    public class Query : IRequest<Result<DownstreamFile>>
    {
        public Guid Id { get; set; }
    }

    public class Handler(IPayrollDownstreamClient payrollClient) : IRequestHandler<Query, Result<DownstreamFile>>
    {
        public Task<Result<DownstreamFile>> Handle(Query request, CancellationToken cancellationToken)
            => payrollClient.DownloadPayslipPdfAsync(request.Id, cancellationToken);
    }
}

public class DownloadMobilePayslipPdfEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(MobileApiConstants.Url.PayslipDownload,
                [Authorize] async (Guid id, ISender sender) =>
                {
                    var result = await sender.Send(new DownloadMobilePayslipPdf.Query { Id = id });
                    if (result.IsSuccess && result.Data != null)
                    {
                        return Results.File(result.Data.Content, result.Data.ContentType, result.Data.FileName);
                    }
                    return result.ToIResult();
                })
            .WithTags(MobileApiConstants.Tags.Payslips)
            .WithName("MobilePayslipDownload")
            .WithSummary("Mobile ESS payslip PDF download");
    }
}
