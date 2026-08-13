using System.Text.Json;

namespace QimErp.Proxy.Mobile.WebApi.Features.Auth;

public static class CompleteTwoFactorMobile
{
    public class Command : IRequest<Result<JsonElement>>
    {
        public string UserId { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Code).NotEmpty();
        }
    }

    public class Handler(IIamDownstreamClient iamClient) : IRequestHandler<Command, Result<JsonElement>>
    {
        public Task<Result<JsonElement>> Handle(Command request, CancellationToken cancellationToken)
            => iamClient.CompleteTwoFactorLoginAsync(new
            {
                request.UserId,
                request.Code
            }, cancellationToken);
    }
}

public class CompleteTwoFactorMobileEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(MobileApiConstants.Url.AuthCompleteTwoFactor,
                [AllowAnonymous] async ([FromBody] CompleteTwoFactorMobile.Command command, ISender sender) =>
                    (await sender.Send(command)).ToIResult())
            .WithTags(MobileApiConstants.Tags.Auth)
            .WithName("MobileCompleteTwoFactor")
            .WithSummary("Mobile ESS complete 2FA login");
    }
}
