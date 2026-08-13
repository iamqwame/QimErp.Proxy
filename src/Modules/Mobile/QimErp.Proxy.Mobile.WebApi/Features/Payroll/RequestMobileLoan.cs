using System.Text.Json;

namespace QimErp.Proxy.Mobile.WebApi.Features.Payroll;

public static class RequestMobileLoan
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
            return payrollClient.RequestMyLoanAsync(body, cancellationToken);
        }
    }
}

public class RequestMobileLoanEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(MobileApiConstants.Url.LoansRequest,
                [Authorize] async (HttpRequest httpRequest, ISender sender) =>
                {
                    using var doc = await JsonDocument.ParseAsync(httpRequest.Body);
                    var command = new RequestMobileLoan.Command { Body = doc.RootElement.Clone() };
                    return (await sender.Send(command)).ToIResult();
                })
            .WithTags(MobileApiConstants.Tags.Payslips)
            .WithName("MobileRequestLoan")
            .WithSummary("Mobile ESS apply for a staff loan");
    }
}
