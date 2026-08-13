using System.Text.Json;

namespace QimErp.Proxy.Mobile.WebApi.Features.Performance;

public static class GetMobileGoals
{
    public class Query : IRequest<Result<JsonElement>> { }

    public class Handler(
        IPerformanceDownstreamClient performanceClient,
        ICurrentEmployeeResolver currentEmployeeResolver)
        : IRequestHandler<Query, Result<JsonElement>>
    {
        public async Task<Result<JsonElement>> Handle(Query request, CancellationToken cancellationToken)
        {
            var employeeId = await currentEmployeeResolver.ResolveAsync(cancellationToken);
            if (employeeId is null)
            {
                return Result.WithFailure<JsonElement>(
                    new Error("GetMobileGoals.NoEmployee", "Employee id could not be resolved."),
                    code: "400");
            }

            return await performanceClient.GetGoalsAsync(employeeId.Value, cancellationToken);
        }
    }
}

public class GetMobileGoalsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(MobileApiConstants.Url.PerformanceGoals,
                [Authorize] async (ISender sender) =>
                    (await sender.Send(new GetMobileGoals.Query())).ToIResult())
            .WithTags(MobileApiConstants.Tags.Performance)
            .WithName("MobilePerformanceGoals")
            .WithSummary("Mobile ESS my performance goals");
    }
}
