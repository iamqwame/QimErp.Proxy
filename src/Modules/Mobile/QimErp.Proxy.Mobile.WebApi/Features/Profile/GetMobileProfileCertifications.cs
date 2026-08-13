namespace QimErp.Proxy.Mobile.WebApi.Features.Profile;

public static class GetMobileProfileCertifications
{
    public class Query : IRequest<Result<List<CertificationResponse>>> { }

    public class Handler(
        IPeopleDownstreamClient peopleClient,
        ICurrentEmployeeResolver currentEmployeeResolver)
        : IRequestHandler<Query, Result<List<CertificationResponse>>>
    {
        public async Task<Result<List<CertificationResponse>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var employeeId = await currentEmployeeResolver.ResolveAsync(cancellationToken);
            if (employeeId is null)
            {
                return Result.WithFailure<List<CertificationResponse>>(
                    new Error("GetMobileProfileCertifications.NoEmployee", "Employee id could not be resolved."),
                    code: "400");
            }

            return await peopleClient.GetCertificationsAsync(employeeId.Value, cancellationToken);
        }
    }
}

public class GetMobileProfileCertificationsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(MobileApiConstants.Url.ProfileCertifications,
                [Authorize] async (ISender sender) =>
                    (await sender.Send(new GetMobileProfileCertifications.Query())).ToIResult())
            .WithTags(MobileApiConstants.Tags.Profile)
            .WithName("MobileProfileCertifications")
            .WithSummary("Mobile ESS profile professional certifications");
    }
}
