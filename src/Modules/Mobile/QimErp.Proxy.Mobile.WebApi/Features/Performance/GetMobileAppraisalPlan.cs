using System.Text.Json;

namespace QimErp.Proxy.Mobile.WebApi.Features.Performance;

public static class GetMobileAppraisalPlan
{
    public class Query : IRequest<Result<JsonElement>>
    {
        public Guid Id { get; set; }
    }

    public class Handler(IPerformanceDownstreamClient performanceClient) : IRequestHandler<Query, Result<JsonElement>>
    {
        public Task<Result<JsonElement>> Handle(Query request, CancellationToken cancellationToken)
            => performanceClient.GetAppraisalPlanAsync(request.Id, cancellationToken);
    }
}

public class GetMobileAppraisalPlanEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(MobileApiConstants.Url.PerformanceAppraisalPlan,
                [Authorize] async (Guid id, ISender sender) =>
                {
                    var query = new GetMobileAppraisalPlan.Query { Id = id };
                    return (await sender.Send(query)).ToIResult();
                })
            .WithTags(MobileApiConstants.Tags.Performance)
            .WithName("MobilePerformanceAppraisalPlan")
            .WithSummary("Mobile ESS appraisal plan detail");
    }
}
