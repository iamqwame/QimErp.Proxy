namespace QimErp.Proxy.Mobile.WebApi.Features.Profile;

public static class GetMobileProfileEmergencyContacts
{
    public class Query : IRequest<Result<PaginatedList<EmergencyContactResponse>>> { }

    public class Handler(
        IPeopleDownstreamClient peopleClient,
        ICurrentEmployeeResolver currentEmployeeResolver)
        : IRequestHandler<Query, Result<PaginatedList<EmergencyContactResponse>>>
    {
        public async Task<Result<PaginatedList<EmergencyContactResponse>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var employeeId = await currentEmployeeResolver.ResolveAsync(cancellationToken);
            if (employeeId is null)
            {
                return Result.WithFailure<PaginatedList<EmergencyContactResponse>>(
                    new Error("GetMobileProfileEmergencyContacts.NoEmployee", "Employee id could not be resolved."),
                    code: "400");
            }

            return await peopleClient.GetEmergencyContactsAsync(
                employeeId.Value,
                new { pageNumber = 1, pageSize = 50 },
                cancellationToken);
        }
    }
}

public class GetMobileProfileEmergencyContactsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(MobileApiConstants.Url.ProfileEmergencyContacts,
                [Authorize] async (ISender sender) =>
                    (await sender.Send(new GetMobileProfileEmergencyContacts.Query())).ToIResult())
            .WithTags(MobileApiConstants.Tags.Profile)
            .WithName("MobileProfileEmergencyContacts")
            .WithSummary("Mobile ESS profile emergency contacts");
    }
}
