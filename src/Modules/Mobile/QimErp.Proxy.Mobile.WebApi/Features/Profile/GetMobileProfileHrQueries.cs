namespace QimErp.Proxy.Mobile.WebApi.Features.Profile;

public static class GetMobileProfileHrQueries
{
    public class Query : IRequest<Result<PaginatedList<HrQueryResponse>>> { }

    public class Handler(
        IPeopleDownstreamClient peopleClient,
        ICurrentEmployeeResolver currentEmployeeResolver)
        : IRequestHandler<Query, Result<PaginatedList<HrQueryResponse>>>
    {
        public async Task<Result<PaginatedList<HrQueryResponse>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var employeeId = await currentEmployeeResolver.ResolveAsync(cancellationToken);
            if (employeeId is null)
            {
                return Result.WithFailure<PaginatedList<HrQueryResponse>>(
                    new Error("GetMobileProfileHrQueries.NoEmployee", "Employee id could not be resolved."),
                    code: "400");
            }

            return await peopleClient.GetMyQueriesAsync(
                employeeId.Value,
                new { pageNumber = 1, pageSize = 50 },
                cancellationToken);
        }
    }
}

public class GetMobileProfileHrQueriesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(MobileApiConstants.Url.ProfileHrQueries,
                [Authorize] async (ISender sender) =>
                    (await sender.Send(new GetMobileProfileHrQueries.Query())).ToIResult())
            .WithTags(MobileApiConstants.Tags.Profile)
            .WithName("MobileProfileHrQueries")
            .WithSummary("Mobile ESS my HR queries");
    }
}
