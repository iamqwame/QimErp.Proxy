namespace QimErp.Proxy.Mobile.WebApi.Features.TimeOff;

public static class CalculateMobileTimeOff
{
    public class Query : IRequest<Result<JsonElement>>
    {
        public Guid LeaveTypeId { get; set; }
        public DateTime? AsOfDate { get; set; }
        public decimal? PlannedDays { get; set; }
        public Guid? EmployeeId { get; set; }
    }

    public class Handler(ILeaveDownstreamClient leaveClient) : IRequestHandler<Query, Result<JsonElement>>
    {
        public Task<Result<JsonElement>> Handle(Query request, CancellationToken cancellationToken)
        {
            var asOf = request.AsOfDate ?? DateTime.UtcNow;
            var qs = $"?leaveTypeId={request.LeaveTypeId}&asOfDate={asOf:yyyy-MM-dd}";
            if (request.PlannedDays.HasValue)
                qs += $"&plannedDays={request.PlannedDays.Value}";
            if (request.EmployeeId.HasValue)
                qs += $"&employeeId={request.EmployeeId.Value}";
            return leaveClient.CalculateAsync(qs, cancellationToken);
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
                    decimal? plannedDays,
                    Guid? employeeId,
                    ISender sender) =>
                {
                    var query = new CalculateMobileTimeOff.Query
                    {
                        LeaveTypeId = leaveTypeId,
                        AsOfDate = asOfDate,
                        PlannedDays = plannedDays,
                        EmployeeId = employeeId
                    };
                    return (await sender.Send(query)).ToIResult();
                })
            .WithTags(MobileApiConstants.Tags.TimeOff)
            .WithName("MobileTimeOffCalculate")
            .WithSummary("Mobile ESS leave balance calculation for a leave type");
    }
}
