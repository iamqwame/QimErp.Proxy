using System.Text.Json;

namespace QimErp.Proxy.Mobile.WebApi.Features.Profile;

public static class GetMobileProfileNextOfKin
{
    public class Query : IRequest<Result<JsonElement>>;

    public class Handler(IPeopleDownstreamClient peopleClient)
        : IRequestHandler<Query, Result<JsonElement>>
    {
        public Task<Result<JsonElement>> Handle(Query request, CancellationToken cancellationToken)
            => peopleClient.GetEssNextOfKinsAsync(cancellationToken);
    }
}

public class GetMobileProfileNextOfKinEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(MobileApiConstants.Url.ProfileNextOfKin,
                [Authorize] async (ISender sender) =>
                    (await sender.Send(new GetMobileProfileNextOfKin.Query())).ToIResult())
            .WithTags(MobileApiConstants.Tags.Profile)
            .WithName("MobileProfileNextOfKin")
            .WithSummary("Mobile ESS profile next of kin");
    }
}
