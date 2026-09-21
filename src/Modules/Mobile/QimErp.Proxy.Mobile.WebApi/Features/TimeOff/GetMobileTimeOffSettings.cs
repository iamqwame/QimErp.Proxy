using System.Text.Json;

namespace QimErp.Proxy.Mobile.WebApi.Features.TimeOff;

public static class GetMobileTimeOffSettings
{
    public class Query : IRequest<Result<JsonElement>> { }

    public class Handler(ILeaveDownstreamClient leaveClient) : IRequestHandler<Query, Result<JsonElement>>
    {
        public Task<Result<JsonElement>> Handle(Query request, CancellationToken cancellationToken)
            => leaveClient.GetSettingsAsync(cancellationToken);
    }
}

public class GetMobileTimeOffSettingsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(MobileApiConstants.Url.TimeOffSettings,
                [Authorize] async (ISender sender) =>
                    (await sender.Send(new GetMobileTimeOffSettings.Query())).ToIResult())
            .WithTags(MobileApiConstants.Tags.TimeOff)
            .WithName("MobileTimeOffSettings")
            .WithSummary("Mobile ESS time-off settings");
    }
}
