using System.Text.Json;

namespace QimErp.Proxy.Mobile.WebApi.Features.TimeOff;

public static class GetMobileTimeOffHistory
{
    public class Query : IRequest<Result<JsonElement>> { }

    public class Handler(ILeaveDownstreamClient leaveClient) : IRequestHandler<Query, Result<JsonElement>>
    {
        public Task<Result<JsonElement>> Handle(Query request, CancellationToken cancellationToken)
            => leaveClient.GetHistoryAsync(cancellationToken);
    }
}

public class GetMobileTimeOffHistoryEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(MobileApiConstants.Url.TimeOffHistory,
                [Authorize] async (ISender sender) =>
                    (await sender.Send(new GetMobileTimeOffHistory.Query())).ToIResult())
            .WithTags(MobileApiConstants.Tags.TimeOff)
            .WithName("MobileTimeOffHistory");
    }
}
