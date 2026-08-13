using System.Text.Json;

namespace QimErp.Proxy.Mobile.WebApi.Features.Compensation;

public static class GetMobileCompensation
{
    public class Query : IRequest<Result<JsonElement>> { }

    public class Handler(IPayrollDownstreamClient payrollClient) : IRequestHandler<Query, Result<JsonElement>>
    {
        public Task<Result<JsonElement>> Handle(Query request, CancellationToken cancellationToken)
            => payrollClient.GetMySummaryAsync(cancellationToken);
    }
}

public class GetMobileCompensationEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(MobileApiConstants.Url.Compensation,
                [Authorize] async (ISender sender) =>
                    (await sender.Send(new GetMobileCompensation.Query())).ToIResult())
            .WithTags(MobileApiConstants.Tags.Compensation)
            .WithName("MobileCompensation")
            .WithSummary("Mobile ESS compensation summary");
    }
}
