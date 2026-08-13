using System.Text.Json;

namespace QimErp.Proxy.Mobile.WebApi.Features.Approvals;

public static class GetMobilePendingApprovals
{
    public class Query : IRequest<Result<JsonElement>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class Handler(IWorkflowDownstreamClient workflowClient) : IRequestHandler<Query, Result<JsonElement>>
    {
        public Task<Result<JsonElement>> Handle(Query request, CancellationToken cancellationToken)
            => workflowClient.GetPendingAsync(request.Page, request.PageSize, cancellationToken);
    }
}

public class GetMobilePendingApprovalsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(MobileApiConstants.Url.ApprovalsPending,
                [Authorize] async ([AsParameters] GetMobilePendingApprovals.Query query, ISender sender) =>
                    (await sender.Send(query)).ToIResult())
            .WithTags(MobileApiConstants.Tags.Approvals)
            .WithName("MobilePendingApprovals");
    }
}
