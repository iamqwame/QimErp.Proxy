using System.Text.Json;

namespace QimErp.Proxy.Mobile.WebApi.Features.TimeOff;

public static class GetMobileHolidays
{
    public class Query : IRequest<Result<JsonElement>>
    {
        public int? Year { get; set; }
    }

    public class Handler(ILeaveDownstreamClient leaveClient) : IRequestHandler<Query, Result<JsonElement>>
    {
        public Task<Result<JsonElement>> Handle(Query request, CancellationToken cancellationToken)
            => leaveClient.GetHolidaysAsync(request.Year, cancellationToken);
    }
}

public class GetMobileHolidaysEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(MobileApiConstants.Url.TimeOffHolidays,
                [Authorize] async (int? year, ISender sender) =>
                {
                    var query = new GetMobileHolidays.Query { Year = year };
                    return (await sender.Send(query)).ToIResult();
                })
            .WithTags(MobileApiConstants.Tags.TimeOff)
            .WithName("MobileTimeOffHolidays")
            .WithSummary("Mobile ESS company holidays");
    }
}
