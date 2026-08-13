using System.Text.Json;

namespace QimErp.Proxy.Mobile.WebApi.Features.People;

public static class GetMobilePeopleDirectory
{
    public class Command : IRequest<Result<JsonElement>>
    {
        public JsonElement Body { get; set; }
    }

    public class Handler(IPeopleDownstreamClient peopleClient) : IRequestHandler<Command, Result<JsonElement>>
    {
        public Task<Result<JsonElement>> Handle(Command request, CancellationToken cancellationToken)
        {
            object body = request.Body.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null
                ? new { pageNumber = 1, pageSize = 100, searchTerm = (string?)null, filter = "all-employees" }
                : JsonSerializer.Deserialize<object>(request.Body.GetRawText())!;
            return peopleClient.GetDirectoryAsync(body, cancellationToken);
        }
    }
}

public class GetMobilePeopleDirectoryEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(MobileApiConstants.Url.PeopleDirectory,
                [Authorize] async (HttpRequest httpRequest, ISender sender) =>
                {
                    using var doc = await JsonDocument.ParseAsync(httpRequest.Body);
                    var command = new GetMobilePeopleDirectory.Command { Body = doc.RootElement.Clone() };
                    return (await sender.Send(command)).ToIResult();
                })
            .WithTags(MobileApiConstants.Tags.People)
            .WithName("MobilePeopleDirectory")
            .WithSummary("Mobile ESS people directory (search + person cards)");
    }
}
