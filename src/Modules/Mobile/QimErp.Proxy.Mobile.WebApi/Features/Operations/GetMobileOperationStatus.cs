using Microsoft.EntityFrameworkCore;
using QimErp.Proxy.Mobile.Shared.Database;

namespace QimErp.Proxy.Mobile.WebApi.Features.Operations;

public static class GetMobileOperationStatus
{
    public sealed record Query(Guid OperationId) : IRequest<Result<Response>>;

    public sealed record Response(
        Guid OperationId,
        string Status,
        DateTime AcceptedAtUtc,
        DateTime? CompletedAtUtc,
        string? FailureCode);

    public sealed class Handler(ProxyCommandDbContext db) : IRequestHandler<Query, Result<Response>>
    {
        public async Task<Result<Response>> Handle(Query request, CancellationToken cancellationToken)
        {
            var operation = await db.ProxyCommands.AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == request.OperationId, cancellationToken);
            return operation is null
                ? Result.WithNotFound<Response>(new Error("Operation.NotFound", "Operation was not found."))
                : Result.WithSuccess(new Response(
                    operation.Id,
                    operation.Status,
                    operation.AcceptedAtUtc,
                    operation.CompletedAtUtc,
                    operation.FailureCode));
        }
    }
}

public sealed class GetMobileOperationStatusEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet("mobile/operations/{operationId:guid}",
                [Authorize] async (Guid operationId, ISender sender, CancellationToken cancellationToken) =>
                    (await sender.Send(new GetMobileOperationStatus.Query(operationId), cancellationToken)).ToIResult())
            .WithTags("Operations")
            .WithName("MobileGetOperationStatus")
            .WithSummary("Get durable mobile command status");
}
