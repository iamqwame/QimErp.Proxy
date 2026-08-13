using System.Text.Json;

namespace QimErp.Proxy.Mobile.WebApi.Features.Performance;

public static class GetMobilePerformanceEssHome
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
                    new Error("GetMobilePerformanceEssHome.NoEmployee", "Employee id could not be resolved."),
                    code: "400");
            }

            return await performanceClient.GetEssHomeAsync(employeeId.Value, cancellationToken);
        }
    }
}

public class GetMobilePerformanceEssHomeEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(MobileApiConstants.Url.PerformanceEssHome,
                [Authorize] async (ISender sender) =>
                    (await sender.Send(new GetMobilePerformanceEssHome.Query())).ToIResult())
            .WithTags(MobileApiConstants.Tags.Performance)
            .WithName("MobilePerformanceEssHome")
            .WithSummary("Mobile ESS performance home snapshot");
    }
}
