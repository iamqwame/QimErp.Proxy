using System.Text.Json;

namespace QimErp.Proxy.Mobile.WebApi.Features.Performance;

public static class GetMobileDevelopmentPlan
{
    public class Query : IRequest<Result<JsonElement>>
    {
        public Guid Id { get; set; }
    }

    public class Handler(IPerformanceDownstreamClient performanceClient) : IRequestHandler<Query, Result<JsonElement>>
    {
        public Task<Result<JsonElement>> Handle(Query request, CancellationToken cancellationToken)
            => performanceClient.GetDevelopmentPlanAsync(request.Id, cancellationToken);
    }
}

public class GetMobileDevelopmentPlanEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(MobileApiConstants.Url.PerformanceDevelopmentPlan,
                [Authorize] async (Guid id, ISender sender) =>
                {
                    var query = new GetMobileDevelopmentPlan.Query { Id = id };
                    return (await sender.Send(query)).ToIResult();
                })
            .WithTags(MobileApiConstants.Tags.Performance)
            .WithName("MobilePerformanceDevelopmentPlan")
            .WithSummary("Mobile ESS development plan detail");
    }
}
