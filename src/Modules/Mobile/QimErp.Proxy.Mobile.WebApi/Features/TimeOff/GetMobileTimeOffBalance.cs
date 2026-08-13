using System.Text.Json;

namespace QimErp.Proxy.Mobile.WebApi.Features.TimeOff;

public static class GetMobileTimeOffBalance
{
    public class Query : IRequest<Result<JsonElement>> { }

    public class Handler(ILeaveDownstreamClient leaveClient) : IRequestHandler<Query, Result<JsonElement>>
    {
        public Task<Result<JsonElement>> Handle(Query request, CancellationToken cancellationToken)
            => leaveClient.GetBalanceAsync(cancellationToken);
    }
}

public class GetMobileTimeOffBalanceEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(MobileApiConstants.Url.TimeOffBalance,
                [Authorize] async (ISender sender) =>
                    (await sender.Send(new GetMobileTimeOffBalance.Query())).ToIResult())
            .WithTags(MobileApiConstants.Tags.TimeOff)
            .WithName("MobileTimeOffBalance");
    }
}
