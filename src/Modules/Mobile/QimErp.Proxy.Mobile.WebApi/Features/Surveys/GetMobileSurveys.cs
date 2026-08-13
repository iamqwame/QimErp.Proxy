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
    }
}
