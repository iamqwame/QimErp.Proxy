using System.Text.Json;

namespace QimErp.Proxy.Mobile.WebApi.Features.Notifications;

public static class GetMobileNotifications
{
    public class Command : IRequest<Result<JsonElement>>
    {
        public JsonElement Body { get; set; }
    }

    public class Handler(INotificationsDownstreamClient notificationsClient) : IRequestHandler<Command, Result<JsonElement>>
    {
        public Task<Result<JsonElement>> Handle(Command request, CancellationToken cancellationToken)
        {
            object body = request.Body.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null
                ? new { pageNumber = 1, pageSize = 50 }
                : JsonSerializer.Deserialize<object>(request.Body.GetRawText())!;
            return notificationsClient.GetHistoryPageAsync(body, cancellationToken);
        }
    }
}

public class GetMobileNotificationsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(MobileApiConstants.Url.NotificationsPage,
                [Authorize] async (HttpRequest httpRequest, ISender sender) =>
                {
                    using var doc = await JsonDocument.ParseAsync(httpRequest.Body);
                    var command = new GetMobileNotifications.Command { Body = doc.RootElement.Clone() };
                    return (await sender.Send(command)).ToIResult();
                })
            .WithTags(MobileApiConstants.Tags.Notifications)
            .WithName("MobileNotificationsPage")
            .WithSummary("Mobile ESS notification history");
    }
}
