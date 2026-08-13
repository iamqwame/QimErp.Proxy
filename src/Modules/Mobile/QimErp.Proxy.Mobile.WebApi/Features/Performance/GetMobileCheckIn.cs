using System.Text.Json;

namespace QimErp.Proxy.Mobile.WebApi.Features.Performance;

public static class GetMobileCheckIn
{
    public class Query : IRequest<Result<JsonElement>>
    {
        public Guid Id { get; set; }
    }

    public class Handler(IPerformanceDownstreamClient performanceClient) : IRequestHandler<Query, Result<JsonElement>>
    {
        public Task<Result<JsonElement>> Handle(Query request, CancellationToken cancellationToken)
            => performanceClient.GetCheckInAsync(request.Id, cancellationToken);
    }
}

public class GetMobileCheckInEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(MobileApiConstants.Url.PerformanceCheckIn,
                [Authorize] async (Guid id, ISender sender) =>
                {
                    var query = new GetMobileCheckIn.Query { Id = id };
                    return (await sender.Send(query)).ToIResult();
                })
            .WithTags(MobileApiConstants.Tags.Performance)
            .WithName("MobilePerformanceCheckIn")
            .WithSummary("Mobile ESS check-in detail");
    }
}
