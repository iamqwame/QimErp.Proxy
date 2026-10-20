using Microsoft.Extensions.Configuration;
using QimErp.Proxy.Mobile.Shared.Services;

namespace QimErp.Proxy.Mobile.WebApi.Tests.Services;

public sealed class ProxyCommandPayloadProtectorTests
{
    [Fact]
    public void Protect_Unprotect_round_trips_plaintext()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ProxyCommandProtection:Key"] = Convert.ToBase64String(Enumerable.Repeat((byte)7, 32).ToArray()),
            })
            .Build();
        var protector = new ProxyCommandPayloadProtector(configuration);

        const string plaintext = """{"route":"/leave","employeeId":"e1"}""";
        var ciphertext = protector.Protect(plaintext);

        ciphertext.Should().NotBe(plaintext);
        protector.Unprotect(ciphertext).Should().Be(plaintext);
    }

    [Fact]
    public void Protect_requires_configured_key()
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
        var act = () => new ProxyCommandPayloadProtector(configuration);
        act.Should().Throw<InvalidOperationException>().WithMessage("*ProxyCommandProtection:Key*");
    }
}
