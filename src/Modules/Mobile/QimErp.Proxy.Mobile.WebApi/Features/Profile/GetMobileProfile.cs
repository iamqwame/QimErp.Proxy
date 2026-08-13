using System.Text.Json;

namespace QimErp.Proxy.Mobile.WebApi.Features.Profile;

public static class GetMobileProfile
{
    public class Query : IRequest<Result<JsonElement>> { }

    public class Handler(
        IPeopleDownstreamClient peopleClient,
        ICurrentEmployeeResolver currentEmployeeResolver)
        : IRequestHandler<Query, Result<JsonElement>>
    {
        public async Task<Result<JsonElement>> Handle(Query request, CancellationToken cancellationToken)
        {
            var employeeId = await currentEmployeeResolver.ResolveAsync(cancellationToken);
            if (employeeId is null)
            {
                return Result.WithFailure<JsonElement>(
                    new Error("GetMobileProfile.NoEmployee", "Employee id could not be resolved."),
                    code: "400");
            }

            return await peopleClient.GetEmployeeAsync(employeeId.Value, cancellationToken);
        }
    }
}

public class GetMobileProfileEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(MobileApiConstants.Url.Profile,
                [Authorize] async (ISender sender) =>
                    (await sender.Send(new GetMobileProfile.Query())).ToIResult())
            .WithTags(MobileApiConstants.Tags.Profile)
            .WithName("MobileGetProfile")
            .WithSummary("Mobile ESS profile");
    }
}
