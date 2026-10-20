using FluentAssertions;
using Xunit;

namespace QimErp.Proxy.Mobile.WebApi.Tests.Architecture;

/// <summary>
public class ProxyCommandData10GuardTests
{
    [Fact]
    public void DbContext_declares_unique_idempotency_index()
    {
        var path = FindSourceFile("ProxyCommandDbContext.cs", "QimErp.Proxy.Mobile.Shared");
        path.Should().NotBeNull();
        var content = File.ReadAllText(path!);
        content.Should().Contain("DATA-10");
        content.Should().Contain("UX_ProxyCommands_TenantId_IdempotencyKey");
        content.Should().Contain(".IsUnique()");
    }

    [Fact]
    public void AcceptService_returns_existing_on_redelivery()
    {
        var path = FindSourceFile("ProxyCommandAcceptService.cs", "QimErp.Proxy.Mobile.Shared");
        path.Should().NotBeNull();
        var content = File.ReadAllText(path!);
        content.Should().Contain("DATA-10");
        content.Should().Contain("WasAlreadyAccepted");
        content.Should().Contain("IdempotencyKey");
    }

    private static string? FindSourceFile(string fileName, string projectFolder)
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            var project = dir.GetDirectories(projectFolder, SearchOption.AllDirectories)
                .FirstOrDefault(d => !d.FullName.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                                  && !d.FullName.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                                  && !d.Name.EndsWith("Tests", StringComparison.Ordinal));
            if (project is not null)
            {
                var match = project.GetFiles(fileName, SearchOption.AllDirectories).FirstOrDefault();
                if (match is not null) return match.FullName;
            }

            dir = dir.Parent;
        }

        return null;
    }
}
