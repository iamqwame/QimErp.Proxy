namespace QimErp.Proxy.Mobile.WebApi.Features.Auth;

public static class LoginMobile
{
    public class Command : IRequest<Result<JsonElement>>
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Username).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
            RuleFor(x => x.Domain).NotEmpty();
        }
    }

    public class Handler(IIamDownstreamClient iamClient) : IRequestHandler<Command, Result<JsonElement>>
    {
        public Task<Result<JsonElement>> Handle(Command request, CancellationToken cancellationToken)
            => iamClient.LoginAsync(new
            {
                request.Username,
                request.Password,
                request.Domain
            }, cancellationToken);
    }
}

public class LoginMobileEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(MobileApiConstants.Url.AuthLogin,
                [AllowAnonymous] async ([FromBody] LoginMobile.Command command, ISender sender) =>
                    (await sender.Send(command)).ToIResult())
            .WithTags(MobileApiConstants.Tags.Auth)
            .WithName("MobileLogin")
            .WithSummary("Mobile ESS login");
    }
}
