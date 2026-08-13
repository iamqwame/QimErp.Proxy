using System.Text.Json;

namespace QimErp.Proxy.Mobile.WebApi.Features.Security;

public static class RevokeMobileSession
{
    public class Command : IRequest<Result<JsonElement>>
    {
        public string SessionId { get; set; } = string.Empty;
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.SessionId).NotEmpty();
        }
    }

    public class Handler(IIamDownstreamClient iamClient) : IRequestHandler<Command, Result<JsonElement>>
    {
        public Task<Result<JsonElement>> Handle(Command request, CancellationToken cancellationToken)
            => iamClient.RevokeSessionAsync(new { request.SessionId }, cancellationToken);
    }
}

public class RevokeMobileSessionEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(MobileApiConstants.Url.SecuritySessionsRevoke,
                [Authorize] async ([FromBody] RevokeMobileSession.Command command, ISender sender) =>
                    (await sender.Send(command)).ToIResult())
            .WithTags(MobileApiConstants.Tags.Security)
            .WithName("MobileSecurityRevokeSession")
            .WithSummary("Mobile ESS revoke a session");
    }
}
