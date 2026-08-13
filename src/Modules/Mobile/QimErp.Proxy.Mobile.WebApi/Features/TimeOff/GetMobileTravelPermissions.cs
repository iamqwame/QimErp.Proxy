using System.Text.Json;

namespace QimErp.Proxy.Mobile.WebApi.Features.TimeOff;

public static class GetMobileTravelPermissions
{
    public class Command : IRequest<Result<JsonElement>>
    {
        public JsonElement Body { get; set; }
    }

    public class Handler(ILeaveDownstreamClient leaveClient) : IRequestHandler<Command, Result<JsonElement>>
    {
        public Task<Result<JsonElement>> Handle(Command request, CancellationToken cancellationToken)
        {
            object body = request.Body.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null
                ? new { pageNumber = 1, pageSize = 50 }
                : JsonSerializer.Deserialize<object>(request.Body.GetRawText())!;
            return leaveClient.GetTravelPermissionsAsync(body, cancellationToken);
        }
    }
}

public class GetMobileTravelPermissionsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(MobileApiConstants.Url.TimeOffTravelMy,
                [Authorize] async (HttpRequest httpRequest, ISender sender) =>
                {
                    using var doc = await JsonDocument.ParseAsync(httpRequest.Body);
                    var command = new GetMobileTravelPermissions.Command { Body = doc.RootElement.Clone() };
                    return (await sender.Send(command)).ToIResult();
                })
            .WithTags(MobileApiConstants.Tags.TimeOff)
            .WithName("MobileTimeOffTravelMy")
            .WithSummary("Mobile ESS my travel permissions");
    }
}
