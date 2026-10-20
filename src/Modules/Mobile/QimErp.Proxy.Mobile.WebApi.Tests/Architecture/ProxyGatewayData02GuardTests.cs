using FluentAssertions;
using Xunit;

namespace QimErp.Proxy.Mobile.WebApi.Tests.Architecture;

/// <summary>
/// DATA-02: Proxy is a JWT-forward BFF — downstream HTTP is gateway composition, not domain sync.
/// </summary>
public class ProxyGatewayData02GuardTests
{
    [Fact]
    public void DownstreamHttpClientBase_forwards_caller_jwt_only()
    {
        var path = FindSourceFile("DownstreamHttpClientBase.cs", "QimErp.Proxy.Mobile.Shared");
        path.Should().NotBeNull();
        var content = File.ReadAllText(path!);
        content.Should().Contain("DATA-02");
        content.Should().Contain("ApplyCallerHeaders");
        content.Should().Contain("Authorization");
        content.Should().NotContain("TryAddWithoutValidation(\"X-Internal-Api-Key\"");
        content.Should().NotContain("InternalApiKey");
    }

    [Fact]
    public void All_downstream_clients_inherit_DownstreamHttpClientBase()
    {
        var sharedRoot = FindProjectRoot("QimErp.Proxy.Mobile.Shared");
        sharedRoot.Should().NotBeNull();
        var clientFiles = Directory.GetFiles(
                Path.Combine(sharedRoot!, "Clients"),
                "*DownstreamClient.cs",
                SearchOption.TopDirectoryOnly)
            .Where(p =>
            {
                var name = Path.GetFileName(p);
                // Skip interface-only files (IFooDownstreamClient.cs), keep IamDownstreamClient.cs.
                return !(name.StartsWith('I') && name.Length > 1 && char.IsUpper(name[1]));
            })
            .ToList();

        clientFiles.Should().NotBeEmpty();
        foreach (var file in clientFiles)
        {
            File.ReadAllText(file).Should().Contain(
                "DownstreamHttpClientBase",
                because: $"{Path.GetFileName(file)} must JWT-forward via the shared gateway base");
        }
    }

    private static string? FindProjectRoot(string projectFolder)
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            var project = dir.GetDirectories(projectFolder, SearchOption.AllDirectories)
                .FirstOrDefault(d => !d.FullName.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                                  && !d.FullName.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                                  && !d.Name.EndsWith("Tests", StringComparison.Ordinal));
            if (project is not null)
                return project.FullName;

            dir = dir.Parent;
        }

        return null;
    }

    private static string? FindSourceFile(string fileName, string projectFolder)
    {
        var root = FindProjectRoot(projectFolder);
        if (root is null) return null;
        return Directory.GetFiles(root, fileName, SearchOption.AllDirectories).FirstOrDefault();
    }
}
