using System.Text.Json;

namespace QimErp.Proxy.Mobile.WebApi.Features.Payslips;

public static class GetMobilePayslipsPage
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
                ? new { pageNumber = 1, pageSize = 20 }
                : JsonSerializer.Deserialize<object>(request.Body.GetRawText())!;
            return payrollClient.GetPayslipsPageAsync(body, cancellationToken);
        }
    }
}

public class GetMobilePayslipsPageEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(MobileApiConstants.Url.PayslipsPage,
                [Authorize] async (HttpRequest httpRequest, ISender sender) =>
                {
                    using var doc = await JsonDocument.ParseAsync(httpRequest.Body);
                    var command = new GetMobilePayslipsPage.Command { Body = doc.RootElement.Clone() };
                    return (await sender.Send(command)).ToIResult();
                })
            .WithTags(MobileApiConstants.Tags.Payslips)
            .WithName("MobilePayslipsPage");
    }
}
