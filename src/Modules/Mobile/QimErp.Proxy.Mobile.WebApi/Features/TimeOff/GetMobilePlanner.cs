using System.Text.Json;

namespace QimErp.Proxy.Mobile.WebApi.Features.TimeOff;

public static class GetMobilePlanner
{
    public class Query : IRequest<Result<JsonElement>>
    {
        public string? Scope { get; set; }
    }

    public class Handler(ILeaveDownstreamClient leaveClient) : IRequestHandler<Query, Result<JsonElement>>
    {
        public Task<Result<JsonElement>> Handle(Query request, CancellationToken cancellationToken)
            => leaveClient.GetPlannerAsync(string.IsNullOrWhiteSpace(request.Scope) ? "mine" : request.Scope, cancellationToken);
    }
}

public class GetMobilePlannerEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(MobileApiConstants.Url.TimeOffPlanner,
                [Authorize] async (string? scope, ISender sender) =>
                {
                    var query = new GetMobilePlanner.Query { Scope = scope };
                    return (await sender.Send(query)).ToIResult();
                })
            .WithTags(MobileApiConstants.Tags.TimeOff)
            .WithName("MobileTimeOffPlanner")
            .WithSummary("Mobile ESS leave planner requests");
    }
}
