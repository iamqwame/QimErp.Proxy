using FluentAssertions;
using Xunit;

namespace QimErp.Proxy.Mobile.WebApi.Tests.Architecture;

/// <summary>
public class MobileHomeData27GuardTests
{
    [Fact]
    public void GetMobileHome_batches_downstream_section_lookups()
    {
        var path = FindSourceFile("GetMobileHome.cs", "QimErp.Proxy.Mobile.WebApi");
        path.Should().NotBeNull();
        var content = File.ReadAllText(path!);
        content.Should().Contain("DATA-27");
        content.Should().Contain("Task.WhenAll");
        content.Should().Contain("GetEssHomeAsync");
        content.Should().Contain("GetBalanceAsync");
        content.Should().Contain("GetPendingAsync");
        content.Should().Contain("GetMySummaryAsync");
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
