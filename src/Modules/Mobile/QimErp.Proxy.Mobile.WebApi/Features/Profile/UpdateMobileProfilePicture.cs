using QimErp.Proxy.Mobile.Shared.Clients;
using QimErp.Proxy.Mobile.WebApi.Services;

namespace QimErp.Proxy.Mobile.WebApi.Features.Profile;

public static class UpdateMobileProfilePicture
{
    public class Command : IRequest<Result<JsonElement>>
    {
        public required string FileName { get; init; }
        public required string ContentType { get; init; }
        public required Func<Stream> OpenRead { get; init; }
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
                    new Error("UpdateMobileProfilePicture.NoEmployee", "Employee id could not be resolved."),
                    code: "400");
            }

            await using var stream = request.OpenRead();
            return await peopleClient.UpdateProfilePictureAsync(
                employeeId.Value,
                stream,
                request.FileName,
                request.ContentType,
                cancellationToken);
        }
    }
}

public class UpdateMobileProfilePictureEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut(MobileApiConstants.Url.ProfilePicture,
                [Authorize] async (HttpRequest httpRequest, ISender sender) =>
                {
                    if (!httpRequest.HasFormContentType)
                    {
                        return Results.BadRequest(new { IsSuccess = false, Message = "Multipart form data required." });
                    }

                    var form = await httpRequest.ReadFormAsync();
                    var file = form.Files["file"];
                    if (file is null || file.Length == 0)
                    {
                        return Results.BadRequest(new { IsSuccess = false, Message = "A file is required." });
                    }

                    var command = new UpdateMobileProfilePicture.Command
                    {
                        FileName = file.FileName,
                        ContentType = string.IsNullOrWhiteSpace(file.ContentType)
                            ? "image/jpeg"
                            : file.ContentType,
                        OpenRead = file.OpenReadStream,
                    };

                    return (await sender.Send(command)).ToIResult();
                })
            .DisableAntiforgery()
            .WithTags(MobileApiConstants.Tags.Profile)
            .WithName("MobileUpdateProfilePicture")
            .WithSummary("Mobile ESS upload profile picture");
    }
}
