using System.Text.Json;

namespace QimErp.Proxy.Mobile.WebApi.Features.Performance;

public static class GetMobileReviewSummary
{
    public class Query : IRequest<Result<JsonElement>>
    {
        public Guid Id { get; set; }
    }

    public class Handler(IPerformanceDownstreamClient performanceClient) : IRequestHandler<Query, Result<JsonElement>>
    {
        public Task<Result<JsonElement>> Handle(Query request, CancellationToken cancellationToken)
            => performanceClient.GetReviewSummaryAsync(request.Id, cancellationToken);
    }
}

public class GetMobileReviewSummaryEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(MobileApiConstants.Url.PerformanceReviewSummary,
                [Authorize] async (Guid id, ISender sender) =>
                {
                    var query = new GetMobileReviewSummary.Query { Id = id };
                    return (await sender.Send(query)).ToIResult();
                })
            .WithTags(MobileApiConstants.Tags.Performance)
            .WithName("MobilePerformanceReviewSummary")
            .WithSummary("Mobile ESS performance review summary");
    }
}
