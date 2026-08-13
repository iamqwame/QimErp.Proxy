using System.Text.Json;

namespace QimErp.Proxy.Mobile.WebApi.Features.TimeOff;

public static class GetMobileTimeOffConfigured
{
    public class Query : IRequest<Result<JsonElement>> { }

    public class Handler(ILeaveDownstreamClient leaveClient) : IRequestHandler<Query, Result<JsonElement>>
    {
        public Task<Result<JsonElement>> Handle(Query request, CancellationToken cancellationToken)
            => leaveClient.GetConfiguredAsync(cancellationToken);
    }
}

public class GetMobileTimeOffConfiguredEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(MobileApiConstants.Url.TimeOffConfigured,
                [Authorize] async (ISender sender) =>
                    (await sender.Send(new GetMobileTimeOffConfigured.Query())).ToIResult())
            .WithTags(MobileApiConstants.Tags.TimeOff)
            .WithName("MobileTimeOffConfigured")
            .WithSummary("Mobile ESS configured leave types and settings");
    }
}
