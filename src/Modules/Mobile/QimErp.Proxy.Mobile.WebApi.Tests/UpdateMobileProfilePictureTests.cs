using System.Text;
using System.Text.Json;
using FluentAssertions;
using Moq;
using QFace.Sdk.Extensions;
using QimErp.Proxy.Mobile.Shared.Clients;
using QimErp.Proxy.Mobile.WebApi.Features.Profile;
using QimErp.Proxy.Mobile.WebApi.Services;

namespace QimErp.Proxy.Mobile.WebApi.Tests;

public class UpdateMobileProfilePictureTests
{
    [Fact]
    public async Task Handle_resolves_employee_and_forwards_stream()
    {
        var people = new Mock<IPeopleDownstreamClient>();
        var currentUser = new Mock<ICurrentEmployeeResolver>();
        var employeeId = Guid.NewGuid();
        var bytes = Encoding.UTF8.GetBytes("fake-image-bytes");
        currentUser.Setup(x => x.ResolveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid?)employeeId);
        people.Setup(x => x.UpdateProfilePictureAsync(
                employeeId,
                It.IsAny<Stream>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.WithSuccess(JsonDocument.Parse("""{"id":"e1"}""").RootElement));

        var handler = new UpdateMobileProfilePicture.Handler(people.Object, currentUser.Object);

        var result = await handler.Handle(
            new UpdateMobileProfilePicture.Command
            {
                FileName = "photo.jpg",
                ContentType = "image/jpeg",
                OpenRead = () => new MemoryStream(bytes),
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        people.Verify(
            x => x.UpdateProfilePictureAsync(
                employeeId,
                It.IsAny<Stream>(),
                "photo.jpg",
                "image/jpeg",
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_returns_error_when_employee_cannot_be_resolved()
    {
        var people = new Mock<IPeopleDownstreamClient>();
        var currentUser = new Mock<ICurrentEmployeeResolver>();
        currentUser.Setup(x => x.ResolveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid?)null);

        var handler = new UpdateMobileProfilePicture.Handler(people.Object, currentUser.Object);

        var result = await handler.Handle(
            new UpdateMobileProfilePicture.Command
            {
                FileName = "photo.jpg",
                ContentType = "image/jpeg",
                OpenRead = () => new MemoryStream(new byte[] { 1 }),
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        people.Verify(
            x => x.UpdateProfilePictureAsync(
                It.IsAny<Guid>(),
                It.IsAny<Stream>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
