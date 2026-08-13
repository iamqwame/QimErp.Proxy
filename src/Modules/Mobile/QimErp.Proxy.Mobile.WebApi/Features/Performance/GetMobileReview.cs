using System.Text.Json;

namespace QimErp.Proxy.Mobile.WebApi.Features.Performance;

public static class GetMobileReview
{
    public class Query : IRequest<Result<JsonElement>>
    {
        public Guid Id { get; set; }
    }

    public class Handler(IPerformanceDownstreamClient performanceClient) : IRequestHandler<Query, Result<JsonElement>>
    {
        public Task<Result<JsonElement>> Handle(Query request, CancellationToken cancellationToken)
            => performanceClient.GetReviewAsync(request.Id, cancellationToken);
    }
}

public class GetMobileReviewEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(MobileApiConstants.Url.PerformanceReview,
                [Authorize] async (Guid id, ISender sender) =>
                {
                    var query = new GetMobileReview.Query { Id = id };
                    return (await sender.Send(query)).ToIResult();
                })
            .WithTags(MobileApiConstants.Tags.Performance)
            .WithName("MobilePerformanceReview")
            .WithSummary("Mobile ESS performance review detail");
    }
}
