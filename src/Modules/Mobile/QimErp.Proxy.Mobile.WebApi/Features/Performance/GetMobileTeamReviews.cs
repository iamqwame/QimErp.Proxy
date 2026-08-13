using System.Text.Json;

namespace QimErp.Proxy.Mobile.WebApi.Features.Performance;

public static class GetMobileTeamReviews
{
    public class Command : IRequest<Result<JsonElement>>
    {
        public JsonElement Body { get; set; }
    }

    public class Handler(
        IPerformanceDownstreamClient performanceClient,
        ICurrentEmployeeResolver currentEmployeeResolver)
        : IRequestHandler<Command, Result<JsonElement>>
    {
        public async Task<Result<JsonElement>> Handle(Command request, CancellationToken cancellationToken)
        {
            var employeeId = await currentEmployeeResolver.ResolveAsync(cancellationToken);
            if (employeeId is null)
            {
                return Result.WithFailure<JsonElement>(
                    new Error("GetMobileTeamReviews.NoEmployee", "Employee id could not be resolved."),
                    code: "400");
            }

            object body = request.Body.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null
                ? new { }
                : JsonSerializer.Deserialize<object>(request.Body.GetRawText())!;

            return await performanceClient.GetTeamReviewsAsync(employeeId.Value, body, cancellationToken);
        }
    }
}

public class GetMobileTeamReviewsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(MobileApiConstants.Url.PerformanceTeamReviews,
                [Authorize] async (HttpRequest httpRequest, ISender sender) =>
                {
                    using var doc = await JsonDocument.ParseAsync(httpRequest.Body);
                    var command = new GetMobileTeamReviews.Command { Body = doc.RootElement.Clone() };
                    return (await sender.Send(command)).ToIResult();
                })
            .WithTags(MobileApiConstants.Tags.Performance)
            .WithName("MobilePerformanceTeamReviews")
            .WithSummary("Mobile ESS team reviews (reviews assigned to me as reviewer)");
    }
}
