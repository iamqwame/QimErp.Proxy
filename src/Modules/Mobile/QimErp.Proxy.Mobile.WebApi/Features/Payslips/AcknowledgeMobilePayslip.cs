using System.Text.Json;

namespace QimErp.Proxy.Mobile.WebApi.Features.Payslips;

public static class AcknowledgeMobilePayslip
{
    public class Command : IRequest<Result<JsonElement>>
    {
        public Guid Id { get; set; }
    }

    public class Handler(IPayrollDownstreamClient payrollClient) : IRequestHandler<Command, Result<JsonElement>>
    {
        public Task<Result<JsonElement>> Handle(Command request, CancellationToken cancellationToken)
            => payrollClient.AcknowledgePayslipAsync(request.Id, cancellationToken);
    }
}

public class AcknowledgeMobilePayslipEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(MobileApiConstants.Url.PayslipAcknowledge,
                [Authorize] async (Guid id, ISender sender) =>
                {
                    var command = new AcknowledgeMobilePayslip.Command { Id = id };
                    return (await sender.Send(command)).ToIResult();
                })
            .WithTags(MobileApiConstants.Tags.Payslips)
            .WithName("MobilePayslipAcknowledge")
            .WithSummary("Mobile ESS acknowledge payslip");
    }
}
