namespace QimErp.Proxy.Mobile.WebApi.Features.Auth;

public static class RefreshMobile
{
    public class Command : IRequest<Result<JsonElement>>
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.AccessToken).NotEmpty();
            RuleFor(x => x.RefreshToken).NotEmpty();
        }
    }

    public class Handler(IIamDownstreamClient iamClient) : IRequestHandler<Command, Result<JsonElement>>
    {
        public Task<Result<JsonElement>> Handle(Command request, CancellationToken cancellationToken)
            => iamClient.RefreshAsync(new
            {
                request.AccessToken,
                request.RefreshToken
            }, cancellationToken);
    }
}

public class RefreshMobileEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(MobileApiConstants.Url.AuthRefresh,
                [AllowAnonymous] async ([FromBody] RefreshMobile.Command command, ISender sender) =>
                    (await sender.Send(command)).ToIResult())
            .WithTags(MobileApiConstants.Tags.Auth)
            .WithName("MobileRefresh")
            .WithSummary("Mobile ESS token refresh");
    }
}
