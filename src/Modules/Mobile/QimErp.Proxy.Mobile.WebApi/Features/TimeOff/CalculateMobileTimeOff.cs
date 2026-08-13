namespace QimErp.Proxy.Mobile.WebApi.Features.TimeOff;

public static class CalculateMobileTimeOff
{
    public class Query : IRequest<Result<JsonElement>>
    {
        public Guid LeaveTypeId { get; set; }
        public DateTime? AsOfDate { get; set; }
    }

    public class Handler(ILeaveDownstreamClient leaveClient) : IRequestHandler<Query, Result<JsonElement>>
    {
        public Task<Result<JsonElement>> Handle(Query request, CancellationToken cancellationToken)
        {
            var asOf = request.AsOfDate ?? DateTime.UtcNow;
            return leaveClient.CalculateAsync(
                $"?leaveTypeId={request.LeaveTypeId}&asOfDate={asOf:yyyy-MM-dd}",
                cancellationToken);
        }
    }
}

public class CalculateMobileTimeOffEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(MobileApiConstants.Url.TimeOffCalculate,
                [Authorize] async (
                    Guid leaveTypeId,
                    DateTime? asOfDate,
                    ISender sender) =>
                {
                    var query = new CalculateMobileTimeOff.Query
                    {
                        LeaveTypeId = leaveTypeId,
                        AsOfDate = asOfDate
                    };
                    return (await sender.Send(query)).ToIResult();
                })
            .WithTags(MobileApiConstants.Tags.TimeOff)
            .WithName("MobileTimeOffCalculate")
            .WithSummary("Mobile ESS leave balance calculation for a leave type");
    }
}
