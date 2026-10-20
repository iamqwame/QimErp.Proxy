using FluentAssertions;
using Xunit;

namespace QimErp.Proxy.Mobile.WebApi.Tests.Architecture;

/// <summary>
/// DATA-24: ProxyCommand Accept fails closed on blank TenantId; Shared has no unguarded IQF.
/// </summary>
public class ProxyData24TenantGuardTests
{
    [Fact]
    public void AcceptService_fail_closes_before_tenant_scoped_query()
    {
        var path = FindSourceFile("ProxyCommandAcceptService.cs", "QimErp.Proxy.Mobile.Shared");
        path.Should().NotBeNull();
        var content = File.ReadAllText(path!);
        content.Should().Contain("DATA-24");
        content.Should().Contain("EnsureTenantId");
        content.Should().Contain("IsNullOrWhiteSpace(tenantId)");
        content.IndexOf("EnsureTenantId", StringComparison.Ordinal)
            .Should().BeLessThan(content.IndexOf("ProxyCommands", StringComparison.Ordinal));
    }

    [Fact]
    public void Shared_has_no_IgnoreQueryFilters()
    {
        var sharedRoot = FindProjectRoot("QimErp.Proxy.Mobile.Shared");
        sharedRoot.Should().NotBeNull();
        var sources = Directory.GetFiles(sharedRoot!, "*.cs", SearchOption.AllDirectories)
            .Where(p => !p.Contains($"{Path.DirectorySeparatorChar}Migrations{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                     && !p.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                     && !p.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .ToList();

        sources.Should().NotBeEmpty();
        foreach (var file in sources)
        {
            File.ReadAllText(file).Should().NotContain(
                "IgnoreQueryFilters",
                because: $"{Path.GetFileName(file)} must not bypass the global tenant filter");
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
