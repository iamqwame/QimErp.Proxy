using System.Text.Json;

namespace QimErp.Proxy.Mobile.WebApi.Features.Profile;

public static class GetMobileDependants
{
    public class Query : IRequest<Result<JsonElement>>;

    public class Handler(IPeopleDownstreamClient peopleClient)
        : IRequestHandler<Query, Result<JsonElement>>
    {
        public Task<Result<JsonElement>> Handle(Query request, CancellationToken cancellationToken)
            => peopleClient.GetEssDependantsAsync(cancellationToken);
    }
}

public static class AddMobileDependant
{
    public class Command : IRequest<Result<JsonElement>>
    {
        public JsonElement Body { get; set; }
    }

    public class Handler(IPeopleDownstreamClient peopleClient)
        : IRequestHandler<Command, Result<JsonElement>>
    {
        public Task<Result<JsonElement>> Handle(Command request, CancellationToken cancellationToken)
        {
            object body = request.Body.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null
                ? new { }
                : JsonSerializer.Deserialize<object>(request.Body.GetRawText())!;
            return peopleClient.AddEssDependantAsync(body, cancellationToken);
        }
    }
}

public static class AddMobileNextOfKin
{
    public class Command : IRequest<Result<JsonElement>>
    {
        public JsonElement Body { get; set; }
    }

    public class Handler(IPeopleDownstreamClient peopleClient)
        : IRequestHandler<Command, Result<JsonElement>>
    {
        public Task<Result<JsonElement>> Handle(Command request, CancellationToken cancellationToken)
        {
            object body = request.Body.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null
                ? new { }
                : JsonSerializer.Deserialize<object>(request.Body.GetRawText())!;
            return peopleClient.AddEssNextOfKinAsync(body, cancellationToken);
        }
    }
}

public class MobileEssRelationshipsEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(MobileApiConstants.Url.ProfileDependants,
                [Authorize] async (ISender sender) =>
                    (await sender.Send(new GetMobileDependants.Query())).ToIResult())
            .WithTags(MobileApiConstants.Tags.Profile)
            .WithName("MobileGetDependants");

        app.MapPost(MobileApiConstants.Url.ProfileDependants,
                [Authorize] async (HttpRequest httpRequest, ISender sender) =>
                {
                    using var doc = await JsonDocument.ParseAsync(httpRequest.Body);
                    var command = new AddMobileDependant.Command { Body = doc.RootElement.Clone() };
                    return (await sender.Send(command)).ToIResult();
                })
            .WithTags(MobileApiConstants.Tags.Profile)
            .WithName("MobileAddDependant");

        app.MapPost(MobileApiConstants.Url.ProfileNextOfKin,
                [Authorize] async (HttpRequest httpRequest, ISender sender) =>
                {
                    using var doc = await JsonDocument.ParseAsync(httpRequest.Body);
                    var command = new AddMobileNextOfKin.Command { Body = doc.RootElement.Clone() };
                    return (await sender.Send(command)).ToIResult();
                })
            .WithTags(MobileApiConstants.Tags.Profile)
            .WithName("MobileAddNextOfKin");
    }
}
