using System.Text.Json;

namespace QimErp.Proxy.Mobile.WebApi.Features.Profile;

public static class SubmitMobileLearningChangeRequest
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

            return peopleClient.SubmitEssLearningChangeRequestAsync(body, cancellationToken);
        }
    }
}

public static class GetMobileLearningChangeRequestsMine
{
    public class Query : IRequest<Result<JsonElement>>;

    public class Handler(IPeopleDownstreamClient peopleClient)
        : IRequestHandler<Query, Result<JsonElement>>
    {
        public Task<Result<JsonElement>> Handle(Query request, CancellationToken cancellationToken)
            => peopleClient.GetMyEssLearningChangeRequestsAsync(cancellationToken);
    }
}

public static class CancelMobileLearningChangeRequest
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

            return peopleClient.CancelEssLearningChangeRequestAsync(request.Id, body, cancellationToken);
        }
    }
}

public class MobileEssLearningEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(MobileApiConstants.Url.ProfileLearningChangeRequests,
                [Authorize] async (HttpRequest httpRequest, ISender sender) =>
                {
                    using var doc = await JsonDocument.ParseAsync(httpRequest.Body);
                    var command = new SubmitMobileLearningChangeRequest.Command { Body = doc.RootElement.Clone() };
                    return (await sender.Send(command)).ToIResult();
                })
            .WithTags(MobileApiConstants.Tags.Profile)
            .WithName("MobileSubmitLearningChangeRequest");

        app.MapGet(MobileApiConstants.Url.ProfileLearningChangeRequestsMine,
                [Authorize] async (ISender sender) =>
                    (await sender.Send(new GetMobileLearningChangeRequestsMine.Query())).ToIResult())
            .WithTags(MobileApiConstants.Tags.Profile)
            .WithName("MobileGetLearningChangeRequestsMine");

        app.MapPost(MobileApiConstants.Url.ProfileLearningChangeRequestCancel,
                [Authorize] async (Guid id, HttpRequest httpRequest, ISender sender) =>
                {
                    JsonElement body = default;
                    if (httpRequest.ContentLength is > 0)
                    {
                        using var doc = await JsonDocument.ParseAsync(httpRequest.Body);
                        body = doc.RootElement.Clone();
                    }

                    var command = new CancelMobileLearningChangeRequest.Command
                    {
                        Id = id,
                        Body = body
                    };
                    return (await sender.Send(command)).ToIResult();
                })
            .WithTags(MobileApiConstants.Tags.Profile)
            .WithName("MobileCancelLearningChangeRequest");
    }
}
