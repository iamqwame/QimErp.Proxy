using System.Text.Json;

namespace QimErp.Proxy.Mobile.WebApi.Features.Benefits;

public static class GetMobileBenefits
{
    public class Query : IRequest<Result<JsonElement>> { }

    public class Handler(
        IBenefitDownstreamClient benefitClient,
        ICurrentEmployeeResolver currentEmployeeResolver)
        : IRequestHandler<Query, Result<JsonElement>>
    {
        public async Task<Result<JsonElement>> Handle(Query request, CancellationToken cancellationToken)
        {
            var employeeId = await currentEmployeeResolver.ResolveAsync(cancellationToken);
            if (employeeId is null)
            {
                return Result.WithFailure<JsonElement>(
                    new Error("GetMobileBenefits.NoEmployee", "Employee id could not be resolved."),
                    code: "400");
            }

            return await benefitClient.GetEmployeeEnrollmentsAsync(employeeId.Value, activeOnly: true, cancellationToken);
        }
    }
}

public class GetMobileBenefitsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(MobileApiConstants.Url.Benefits,
                [Authorize] async (ISender sender) =>
                    (await sender.Send(new GetMobileBenefits.Query())).ToIResult())
            .WithTags(MobileApiConstants.Tags.Benefits)
            .WithName("MobileBenefits")
            .WithSummary("Mobile ESS my benefit enrollments");
    }
}
