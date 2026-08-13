using System.Text.Json;

namespace QimErp.Proxy.Mobile.WebApi.Features.TimeOff;

public static class GetMobileTimeOffMyRequests
{
    public class Query : IRequest<Result<JsonElement>> { }

    public class Handler(ILeaveDownstreamClient leaveClient) : IRequestHandler<Query, Result<JsonElement>>
    {
        public Task<Result<JsonElement>> Handle(Query request, CancellationToken cancellationToken)
            => leaveClient.GetMyRequestsAsync(cancellationToken);
    }
}

public class GetMobileTimeOffMyRequestsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(MobileApiConstants.Url.TimeOffMyRequests,
                [Authorize] async (ISender sender) =>
                    (await sender.Send(new GetMobileTimeOffMyRequests.Query())).ToIResult())
            .WithTags(MobileApiConstants.Tags.TimeOff)
            .WithName("MobileTimeOffMyRequests");
    }
}
