using System.Text.Json;

namespace QimErp.Proxy.Mobile.WebApi.Features.Payroll;

public static class GetMobileClaims
{
    public class Query : IRequest<Result<JsonElement>> { }

    public class Handler(IPayrollDownstreamClient payrollClient) : IRequestHandler<Query, Result<JsonElement>>
    {
        public Task<Result<JsonElement>> Handle(Query request, CancellationToken cancellationToken)
            => payrollClient.GetMyClaimsAsync(cancellationToken);
    }
}

public class GetMobileClaimsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(MobileApiConstants.Url.PayrollClaims,
                [Authorize] async (ISender sender) =>
                    (await sender.Send(new GetMobileClaims.Query())).ToIResult())
            .WithTags(MobileApiConstants.Tags.Payslips)
            .WithName("MobileMyClaims")
            .WithSummary("Mobile ESS my claims");
    }
}
