using System.Text.Json;

namespace QimErp.Proxy.Mobile.WebApi.Features.Profile;

public static class UpdateMobileProfileContactInfo
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

            return peopleClient.UpdateEssContactInfoAsync(body, cancellationToken);
        }
    }
}

public static class SubmitMobilePersonalChangeRequest
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

            return peopleClient.SubmitEssPersonalChangeRequestAsync(body, cancellationToken);
        }
    }
}

public static class GetMobilePersonalChangeRequestsMine
{
    public class Query : IRequest<Result<JsonElement>>;

    public class Handler(IPeopleDownstreamClient peopleClient)
        : IRequestHandler<Query, Result<JsonElement>>
    {
        public Task<Result<JsonElement>> Handle(Query request, CancellationToken cancellationToken)
            => peopleClient.GetMyEssPersonalChangeRequestsAsync(cancellationToken);
    }
}

public static class CancelMobilePersonalChangeRequest
{
    public class Command : IRequest<Result<JsonElement>>
    {
        public Guid Id { get; set; }
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

            return peopleClient.CancelEssPersonalChangeRequestAsync(request.Id, body, cancellationToken);
        }
    }
}

public class MobileEssProfileEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut(MobileApiConstants.Url.ProfileContactInfo,
                [Authorize] async (HttpRequest httpRequest, ISender sender) =>
                {
                    using var doc = await JsonDocument.ParseAsync(httpRequest.Body);
                    var command = new UpdateMobileProfileContactInfo.Command { Body = doc.RootElement.Clone() };
                    return (await sender.Send(command)).ToIResult();
                })
            .WithTags(MobileApiConstants.Tags.Profile)
            .WithName("MobileUpdateProfileContactInfo")
            .WithSummary("Mobile ESS update contact info (immediate)");

        app.MapPost(MobileApiConstants.Url.ProfilePersonalChangeRequests,
                [Authorize] async (HttpRequest httpRequest, ISender sender) =>
                {
                    using var doc = await JsonDocument.ParseAsync(httpRequest.Body);
                    var command = new SubmitMobilePersonalChangeRequest.Command { Body = doc.RootElement.Clone() };
                    return (await sender.Send(command)).ToIResult();
                })
            .WithTags(MobileApiConstants.Tags.Profile)
            .WithName("MobileSubmitPersonalChangeRequest")
            .WithSummary("Mobile ESS submit personal identity change request");

        app.MapGet(MobileApiConstants.Url.ProfilePersonalChangeRequestsMine,
                [Authorize] async (ISender sender) =>
                    (await sender.Send(new GetMobilePersonalChangeRequestsMine.Query())).ToIResult())
            .WithTags(MobileApiConstants.Tags.Profile)
            .WithName("MobileGetPersonalChangeRequestsMine")
            .WithSummary("Mobile ESS list my personal data change requests");

        app.MapPost(MobileApiConstants.Url.ProfilePersonalChangeRequestCancel,
                [Authorize] async (Guid id, HttpRequest httpRequest, ISender sender) =>
                {
                    JsonElement body = default;
                    if (httpRequest.ContentLength is > 0)
                    {
                        using var doc = await JsonDocument.ParseAsync(httpRequest.Body);
                        body = doc.RootElement.Clone();
                    }

                    var command = new CancelMobilePersonalChangeRequest.Command
                    {
                        Id = id,
                        Body = body
                    };
                    return (await sender.Send(command)).ToIResult();
                })
            .WithTags(MobileApiConstants.Tags.Profile)
            .WithName("MobileCancelPersonalChangeRequest")
            .WithSummary("Mobile ESS cancel pending personal data change request");
    }
}
