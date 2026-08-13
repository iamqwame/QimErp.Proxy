using System.Text.Json;

namespace QimErp.Proxy.Mobile.WebApi.Features.Payroll;

public static class RequestMobileClaim
{
    public class Command : IRequest<Result<JsonElement>>
    {
        public JsonElement Body { get; set; }
    }

    public class Handler(IPayrollDownstreamClient payrollClient) : IRequestHandler<Command, Result<JsonElement>>
    {
        public Task<Result<JsonElement>> Handle(Command request, CancellationToken cancellationToken)
        {
            object body = request.Body.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null
                ? new { }
                : JsonSerializer.Deserialize<object>(request.Body.GetRawText())!;
            return payrollClient.RequestMyClaimAsync(body, cancellationToken);
        }
    }
}

public class RequestMobileClaimEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(MobileApiConstants.Url.PayrollClaimsRequest,
                [Authorize] async (HttpRequest httpRequest, ISender sender) =>
                {
                    using var doc = await JsonDocument.ParseAsync(httpRequest.Body);
                    var command = new RequestMobileClaim.Command { Body = doc.RootElement.Clone() };
                    return (await sender.Send(command)).ToIResult();
                })
            .WithTags(MobileApiConstants.Tags.Payslips)
            .WithName("MobileRequestClaim")
            .WithSummary("Mobile ESS submit a claim");
    }
}
