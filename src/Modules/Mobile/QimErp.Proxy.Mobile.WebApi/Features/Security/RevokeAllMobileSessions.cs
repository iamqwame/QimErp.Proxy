using System.Text.Json;

namespace QimErp.Proxy.Mobile.WebApi.Features.Security;

public static class RevokeAllMobileSessions
{
    public class Command : IRequest<Result<JsonElement>> { }

    public class Handler(IIamDownstreamClient iamClient) : IRequestHandler<Command, Result<JsonElement>>
    {
        public Task<Result<JsonElement>> Handle(Command request, CancellationToken cancellationToken)
            => iamClient.RevokeAllSessionsAsync(cancellationToken);
    }
}

public class RevokeAllMobileSessionsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(MobileApiConstants.Url.SecuritySessionsRevokeAll,
                [Authorize] async (ISender sender) =>
                    (await sender.Send(new RevokeAllMobileSessions.Command())).ToIResult())
            .WithTags(MobileApiConstants.Tags.Security)
            .WithName("MobileSecurityRevokeAllSessions")
            .WithSummary("Mobile ESS revoke all sessions");
    }
}
