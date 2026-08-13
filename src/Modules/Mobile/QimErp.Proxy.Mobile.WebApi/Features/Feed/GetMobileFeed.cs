using System.Text.Json;

namespace QimErp.Proxy.Mobile.WebApi.Features.Feed;

public static class GetMobileFeed
{
    public class Query : IRequest<Result<JsonElement>> { }

    public class Handler(IPeopleDownstreamClient peopleClient)
        : IRequestHandler<Query, Result<JsonElement>>
    {
        public Task<Result<JsonElement>> Handle(Query request, CancellationToken cancellationToken)
            => peopleClient.GetFeedAsync(cancellationToken);
    }
}

public class GetMobileFeedEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(MobileApiConstants.Url.Feed,
                [Authorize] async (ISender sender) =>
                    (await sender.Send(new GetMobileFeed.Query())).ToIResult())
            .WithTags(MobileApiConstants.Tags.Me)
            .WithName("MobileFeed")
            .WithSummary("Mobile ESS company news feed for the current employee");
    }
}
