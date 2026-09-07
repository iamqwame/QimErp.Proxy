using System.Text.Json;

namespace QimErp.Proxy.Mobile.WebApi.Features.Surveys;

public static class GetMobileSurveys
{
    public class Command : IRequest<Result<JsonElement>>
    {
        public JsonElement Body { get; set; }
    }

    public class Handler(ISurveysDownstreamClient surveysClient) : IRequestHandler<Command, Result<JsonElement>>
    {
        public Task<Result<JsonElement>> Handle(Command request, CancellationToken cancellationToken)
        {
            object body = request.Body.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null
                ? new { pageNumber = 1, pageSize = 50 }
                : JsonSerializer.Deserialize<object>(request.Body.GetRawText())!;
            return surveysClient.GetMyResponsesPageAsync(body, cancellationToken);
        }
    }
}

public static class GetMobileAvailableSurveys
{
    public class Command : IRequest<Result<JsonElement>>
    {
        public JsonElement Body { get; set; }
    }

    public class Handler(ISurveysDownstreamClient surveysClient) : IRequestHandler<Command, Result<JsonElement>>
    {
        public Task<Result<JsonElement>> Handle(Command request, CancellationToken cancellationToken)
        {
            object body = request.Body.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null
                ? new { pageNumber = 1, pageSize = 50 }
                : JsonSerializer.Deserialize<object>(request.Body.GetRawText())!;
            return surveysClient.GetAvailablePageAsync(body, cancellationToken);
        }
    }
}

public static class StartMobileSurveyResponse
{
    public class Command : IRequest<Result<JsonElement>>
    {
        public JsonElement Body { get; set; }
    }

    public class Handler(ISurveysDownstreamClient surveysClient) : IRequestHandler<Command, Result<JsonElement>>
    {
        public Task<Result<JsonElement>> Handle(Command request, CancellationToken cancellationToken)
        {
            object body = request.Body.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null
                ? new { }
                : JsonSerializer.Deserialize<object>(request.Body.GetRawText())!;
            return surveysClient.StartResponseAsync(body, cancellationToken);
        }
    }
}

public static class SaveMobileSurveyProgress
{
    public class Command : IRequest<Result<JsonElement>>
    {
        public Guid ResponseId { get; set; }
        public JsonElement Body { get; set; }
    }

    public class Handler(ISurveysDownstreamClient surveysClient) : IRequestHandler<Command, Result<JsonElement>>
    {
        public Task<Result<JsonElement>> Handle(Command request, CancellationToken cancellationToken)
        {
            object body = request.Body.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null
                ? new { answers = Array.Empty<object>() }
                : JsonSerializer.Deserialize<object>(request.Body.GetRawText())!;
            return surveysClient.SaveProgressAsync(request.ResponseId, body, cancellationToken);
        }
    }
}

public static class SubmitMobileSurveyResponse
{
    public class Command : IRequest<Result<JsonElement>>
    {
        public Guid ResponseId { get; set; }
        public JsonElement Body { get; set; }
    }

    public class Handler(ISurveysDownstreamClient surveysClient) : IRequestHandler<Command, Result<JsonElement>>
    {
        public Task<Result<JsonElement>> Handle(Command request, CancellationToken cancellationToken)
        {
            object? body = request.Body.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null
                ? null
                : JsonSerializer.Deserialize<object>(request.Body.GetRawText());
            return surveysClient.SubmitResponseAsync(request.ResponseId, body, cancellationToken);
        }
    }
}

public class GetMobileSurveysEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(MobileApiConstants.Url.SurveysMyResponses,
                [Authorize] async (HttpRequest httpRequest, ISender sender) =>
                {
                    using var doc = await JsonDocument.ParseAsync(httpRequest.Body);
                    var command = new GetMobileSurveys.Command { Body = doc.RootElement.Clone() };
                    return (await sender.Send(command)).ToIResult();
                })
            .WithTags(MobileApiConstants.Tags.Surveys)
            .WithName("MobileSurveysMyResponses")
            .WithSummary("Mobile ESS my survey responses");

        app.MapPost(MobileApiConstants.Url.SurveysAvailable,
                [Authorize] async (HttpRequest httpRequest, ISender sender) =>
                {
                    using var doc = await JsonDocument.ParseAsync(httpRequest.Body);
                    var command = new GetMobileAvailableSurveys.Command { Body = doc.RootElement.Clone() };
                    return (await sender.Send(command)).ToIResult();
                })
            .WithTags(MobileApiConstants.Tags.Surveys)
            .WithName("MobileSurveysAvailable")
            .WithSummary("Mobile ESS available surveys");

        app.MapPost(MobileApiConstants.Url.SurveysStartResponse,
                [Authorize] async (HttpRequest httpRequest, ISender sender) =>
                {
                    using var doc = await JsonDocument.ParseAsync(httpRequest.Body);
                    var command = new StartMobileSurveyResponse.Command { Body = doc.RootElement.Clone() };
                    return (await sender.Send(command)).ToIResult();
                })
            .WithTags(MobileApiConstants.Tags.Surveys)
            .WithName("MobileSurveysStartResponse")
            .WithSummary("Mobile ESS start survey response");

        app.MapPut(MobileApiConstants.Url.SurveysSaveProgress,
                [Authorize] async (Guid id, HttpRequest httpRequest, ISender sender) =>
                {
                    using var doc = await JsonDocument.ParseAsync(httpRequest.Body);
                    var command = new SaveMobileSurveyProgress.Command
                    {
                        ResponseId = id,
                        Body = doc.RootElement.Clone()
                    };
                    return (await sender.Send(command)).ToIResult();
                })
            .WithTags(MobileApiConstants.Tags.Surveys)
            .WithName("MobileSurveysSaveProgress")
            .WithSummary("Mobile ESS save survey progress");

        app.MapPost(MobileApiConstants.Url.SurveysSubmitResponse,
                [Authorize] async (Guid id, HttpRequest httpRequest, ISender sender) =>
                {
                    using var doc = await JsonDocument.ParseAsync(httpRequest.Body);
                    var command = new SubmitMobileSurveyResponse.Command
                    {
                        ResponseId = id,
                        Body = doc.RootElement.Clone()
                    };
                    return (await sender.Send(command)).ToIResult();
                })
            .WithTags(MobileApiConstants.Tags.Surveys)
            .WithName("MobileSurveysSubmitResponse")
            .WithSummary("Mobile ESS submit survey response");
    }
}
