using System.Text.Json;

namespace QimErp.Proxy.Mobile.WebApi.Features.Profile;

public static class GetMobileProfileEducation
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
                    new Error("GetMobileProfileEducation.NoEmployee", "Employee id could not be resolved."),
                    code: "400");
            }

            return await peopleClient.GetQualificationsAsync(employeeId.Value, cancellationToken);
        }
    }
}

public class GetMobileProfileEducationEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(MobileApiConstants.Url.ProfileEducation,
                [Authorize] async (ISender sender) =>
                    (await sender.Send(new GetMobileProfileEducation.Query())).ToIResult())
            .WithTags(MobileApiConstants.Tags.Profile)
            .WithName("MobileProfileEducation")
            .WithSummary("Mobile ESS profile education qualifications");
    }
}
