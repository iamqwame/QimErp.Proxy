using System.Text.Json;

namespace QimErp.Proxy.Mobile.WebApi.Features.Auth;

public static class TenantConfigMobile
{
    public class Query : IRequest<Result<JsonElement>>
    {
        public string Domain { get; set; } = string.Empty;
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Domain).NotEmpty();
        }
    }

    public class Handler(IIamDownstreamClient iamClient) : IRequestHandler<Query, Result<JsonElement>>
    {
        public Task<Result<JsonElement>> Handle(Query request, CancellationToken cancellationToken)
            => iamClient.GetTenantConfigAsync(request.Domain, cancellationToken);
    }
}

public class TenantConfigMobileEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(MobileApiConstants.Url.AuthTenantConfig,
                [AllowAnonymous] async (string domain, ISender sender) =>
                {
                    var query = new TenantConfigMobile.Query { Domain = domain };
                    return (await sender.Send(query)).ToIResult();
                })
            .WithTags(MobileApiConstants.Tags.Auth)
            .WithName("MobileTenantConfig")
            .WithSummary("Mobile ESS tenant config by domain");
    }
}
