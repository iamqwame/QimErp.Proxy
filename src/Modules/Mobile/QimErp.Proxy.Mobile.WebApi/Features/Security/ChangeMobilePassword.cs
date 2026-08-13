using System.Text.Json;

namespace QimErp.Proxy.Mobile.WebApi.Features.Security;

public static class ChangeMobilePassword
{
    public class Command : IRequest<Result<JsonElement>>
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.CurrentPassword).NotEmpty();
            RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8);
        }
    }

    public class Handler(IIamDownstreamClient iamClient) : IRequestHandler<Command, Result<JsonElement>>
    {
        public Task<Result<JsonElement>> Handle(Command request, CancellationToken cancellationToken)
            => iamClient.ChangePasswordAsync(new
            {
                request.CurrentPassword,
                request.NewPassword
            }, cancellationToken);
    }
}

public class ChangeMobilePasswordEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(MobileApiConstants.Url.SecurityChangePassword,
                [Authorize] async ([FromBody] ChangeMobilePassword.Command command, ISender sender) =>
                    (await sender.Send(command)).ToIResult())
            .WithTags(MobileApiConstants.Tags.Security)
            .WithName("MobileSecurityChangePassword")
            .WithSummary("Mobile ESS change own password");
    }
}
