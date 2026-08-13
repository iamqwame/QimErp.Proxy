using System.Text.Json;

namespace QimErp.Proxy.Mobile.WebApi.Features.Profile;

public static class UpdateMobileProfilePersonal
{
    public class Command : IRequest<Result<JsonElement>>
    {
        public JsonElement Body { get; set; }
    }

    public class Handler(
        IPeopleDownstreamClient peopleClient,
        ICurrentEmployeeResolver currentEmployeeResolver)
        : IRequestHandler<Command, Result<JsonElement>>
    {
        public async Task<Result<JsonElement>> Handle(Command request, CancellationToken cancellationToken)
        {
            var employeeId = await currentEmployeeResolver.ResolveAsync(cancellationToken);
            if (employeeId is null)
            {
                return Result.WithFailure<JsonElement>(
                    new Error("UpdateMobileProfilePersonal.NoEmployee", "Employee id could not be resolved."),
                    code: "400");
            }

            object body = request.Body.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null
                ? new { }
                : JsonSerializer.Deserialize<object>(request.Body.GetRawText())!;

            return await peopleClient.UpdatePersonalInfoAsync(employeeId.Value, body, cancellationToken);
        }
    }
}

public class UpdateMobileProfilePersonalEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut(MobileApiConstants.Url.ProfilePersonal,
                [Authorize] async (HttpRequest httpRequest, ISender sender) =>
                {
                    using var doc = await JsonDocument.ParseAsync(httpRequest.Body);
                    var command = new UpdateMobileProfilePersonal.Command { Body = doc.RootElement.Clone() };
                    return (await sender.Send(command)).ToIResult();
                })
            .WithTags(MobileApiConstants.Tags.Profile)
            .WithName("MobileUpdateProfilePersonal")
            .WithSummary("Mobile ESS update personal info");
    }
}
