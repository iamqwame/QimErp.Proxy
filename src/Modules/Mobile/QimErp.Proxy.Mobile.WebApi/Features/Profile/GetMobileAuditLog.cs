namespace QimErp.Proxy.Mobile.WebApi.Features.Profile;

public static class GetMobileAuditLog
{
    public class Query : IRequest<Result<PaginatedList<AuditActivityResponse>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class Handler(IIamDownstreamClient iamClient)
        : IRequestHandler<Query, Result<PaginatedList<AuditActivityResponse>>>
    {
        public Task<Result<PaginatedList<AuditActivityResponse>>> Handle(Query request, CancellationToken cancellationToken)
            => iamClient.GetMyActivitiesAsync(request.PageNumber, request.PageSize, cancellationToken);
    }
}

public class GetMobileAuditLogEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(MobileApiConstants.Url.AuditLog,
                [Authorize] async (ISender sender, int pageNumber = 1, int pageSize = 20) =>
                {
                    var query = new GetMobileAuditLog.Query
                    {
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    };
                    return (await sender.Send(query)).ToIResult();
                })
            .WithTags(MobileApiConstants.Tags.Audit)
            .WithName("MobileAuditLog")
            .WithSummary("Mobile ESS my recent account activity (audit log)");
    }
}
