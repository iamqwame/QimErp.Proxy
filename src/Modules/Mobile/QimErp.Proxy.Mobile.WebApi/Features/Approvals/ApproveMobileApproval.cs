using System.Text.Json;

namespace QimErp.Proxy.Mobile.WebApi.Features.Approvals;

public static class ApproveMobileApproval
{
    public class Command : IRequest<Result<JsonElement>>
    {
        public JsonElement Body { get; set; }
    }

    public class Handler(IWorkflowDownstreamClient workflowClient) : IRequestHandler<Command, Result<JsonElement>>
    {
        public Task<Result<JsonElement>> Handle(Command request, CancellationToken cancellationToken)
        {
            object body = request.Body.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null
                ? new { }
                : JsonSerializer.Deserialize<object>(request.Body.GetRawText())!;
            return workflowClient.ApproveAsync(body, cancellationToken);
        }
    }
}

public class ApproveMobileApprovalEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(MobileApiConstants.Url.ApprovalsApprove,
                [Authorize] async (HttpRequest httpRequest, ISender sender) =>
                {
                    using var doc = await JsonDocument.ParseAsync(httpRequest.Body);
                    var command = new ApproveMobileApproval.Command { Body = doc.RootElement.Clone() };
                    return (await sender.Send(command)).ToIResult();
                })
            .WithTags(MobileApiConstants.Tags.Approvals)
            .WithName("MobileApprove");
    }
}
