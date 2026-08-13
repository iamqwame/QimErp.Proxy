using System.Text.Json;

namespace QimErp.Proxy.Mobile.WebApi.Features.People;

public static class GetMobilePeopleFilters
{
    public class Query : IRequest<Result<JsonElement>> { }

    public class Handler(IPeopleDownstreamClient peopleClient) : IRequestHandler<Query, Result<JsonElement>>
    {
        public Task<Result<JsonElement>> Handle(Query request, CancellationToken cancellationToken)
            => peopleClient.GetFiltersAsync(cancellationToken);
    }
}

public class GetMobilePeopleFiltersEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(MobileApiConstants.Url.PeopleFilters,
                [Authorize] async (ISender sender) =>
                    (await sender.Send(new GetMobilePeopleFilters.Query())).ToIResult())
            .WithTags(MobileApiConstants.Tags.People)
            .WithName("MobilePeopleFilters")
            .WithSummary("Mobile ESS people directory filters");
    }
}
