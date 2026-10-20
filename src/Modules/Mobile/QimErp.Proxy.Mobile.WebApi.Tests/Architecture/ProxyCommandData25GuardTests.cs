using FluentAssertions;
using Xunit;

namespace QimErp.Proxy.Mobile.WebApi.Tests.Architecture;

public class ProxyCommandData25GuardTests
{
    [Fact]
    public void ProxyCommand_classifies_PayloadCiphertext_as_Sensitive()
    {
        var path = FindSourceFile("ProxyCommand.cs", "QimErp.Proxy.Mobile.Shared");
        path.Should().NotBeNull();
        var content = File.ReadAllText(path!);
        content.Should().Contain("DATA-25");
        content.Should().Contain("DataClassification.Sensitive");
        content.Should().Contain("PayloadCiphertext");
    }

    [Fact]
    public void PayloadProtector_requires_configured_key()
    {
        var path = FindSourceFile("ProxyCommandPayloadProtector.cs", "QimErp.Proxy.Mobile.Shared");
        path.Should().NotBeNull();
        var content = File.ReadAllText(path!);
        content.Should().Contain("ProxyCommandProtection:Key");
        content.Should().Contain("InvalidOperationException");
        content.Should().Contain("AesGcm");
    }

    [Fact]
    public void Program_registers_BlindIndex()
    {
        var path = FindSourceFile("Program.cs", "QimErp.Proxy.Mobile.WebApi");
        path.Should().NotBeNull();
        var content = File.ReadAllText(path!);
        content.Should().Contain("AddBlindIndex");
    }

    [Fact]
    public void Appsettings_does_not_commit_Jwt_Secret()
    {
        var path = FindSourceFile("appsettings.json", "QimErp.Proxy.Mobile.WebApi");
        path.Should().NotBeNull();
        var content = File.ReadAllText(path!);
        content.Should().NotContain("\"Secret\"");
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
