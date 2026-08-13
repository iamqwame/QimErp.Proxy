using System.Text.Json;

namespace QimErp.Proxy.Mobile.WebApi.Features.Me;

public static class GetMobileMe
{
    public class Query : IRequest<Result<MobileMeResponse>> { }

    public class Handler(
        IIamDownstreamClient iamClient,
        IPeopleDownstreamClient peopleClient,
        ICurrentEmployeeResolver currentEmployeeResolver)
        : IRequestHandler<Query, Result<MobileMeResponse>>
    {
        public async Task<Result<MobileMeResponse>> Handle(Query request, CancellationToken cancellationToken)
        {
            var degraded = new List<string>();
            JsonElement? user = null;
            JsonElement? employee = null;

            var meResult = await iamClient.GetMeAsync(cancellationToken);
            if (meResult.IsSuccess)
            {
                user = meResult.Data;
            }
            else
            {
                degraded.Add("user");
            }

            var employeeId = await currentEmployeeResolver.ResolveAsync(cancellationToken);
            if (employeeId is { } resolvedEmployeeId)
            {
                var employeeResult = await peopleClient.GetEmployeeAsync(resolvedEmployeeId, cancellationToken);
                if (employeeResult.IsSuccess)
                {
                    employee = employeeResult.Data;
                }
                else
                {
                    degraded.Add("employee");
                }
            }
            else
            {
                degraded.Add("employee");
            }

            return Result.WithSuccess(new MobileMeResponse
            {
                User = user,
                Employee = employee,
                DegradedSections = degraded
            });
        }
    }
}

public class GetMobileMeEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(MobileApiConstants.Url.Me,
                [Authorize] async (ISender sender) =>
                    (await sender.Send(new GetMobileMe.Query())).ToIResult())
            .WithTags(MobileApiConstants.Tags.Me)
            .WithName("MobileMe")
            .WithSummary("Mobile ESS current user + employee");
    }
}
