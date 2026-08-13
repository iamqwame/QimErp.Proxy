namespace QimErp.Proxy.Mobile.WebApi.Features.Profile;

public static class GetMobileProfileTrainings
{
    public class Query : IRequest<Result<List<TrainingResponse>>> { }

    public class Handler(
        IPeopleDownstreamClient peopleClient,
        ICurrentEmployeeResolver currentEmployeeResolver)
        : IRequestHandler<Query, Result<List<TrainingResponse>>>
    {
        public async Task<Result<List<TrainingResponse>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var employeeId = await currentEmployeeResolver.ResolveAsync(cancellationToken);
            if (employeeId is null)
            {
                return Result.WithFailure<List<TrainingResponse>>(
                    new Error("GetMobileProfileTrainings.NoEmployee", "Employee id could not be resolved."),
                    code: "400");
            }

            return await peopleClient.GetTrainingsAsync(employeeId.Value, cancellationToken);
        }
    }
}

public class GetMobileProfileTrainingsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(MobileApiConstants.Url.ProfileTrainings,
                [Authorize] async (ISender sender) =>
                    (await sender.Send(new GetMobileProfileTrainings.Query())).ToIResult())
            .WithTags(MobileApiConstants.Tags.Profile)
            .WithName("MobileProfileTrainings")
            .WithSummary("Mobile ESS profile training & development records");
    }
}
