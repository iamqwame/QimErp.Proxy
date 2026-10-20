using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using QimErp.Shared.Common.Security;

namespace QimErp.Proxy.Mobile.Shared.Security;

public static class BlindIndexServiceCollectionExtensions
{
    // DATA-25: register HMAC blind index for future PII exact-lookup beachheads.
    public static IServiceCollection AddBlindIndex(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<BlindIndexOptions>(configuration.GetSection(BlindIndexOptions.SectionName));
        services.AddSingleton(sp =>
        {
            var opts = sp.GetRequiredService<IOptions<BlindIndexOptions>>().Value;
            var env = sp.GetRequiredService<IHostEnvironment>();
            var config = sp.GetRequiredService<IConfiguration>();
            return new BlindIndex(ResolveKeyBytes(opts.KeyBase64, config, env));
        });
        return services;
    }

    private static byte[] ResolveKeyBytes(string? keyBase64, IConfiguration configuration, IHostEnvironment env)
    {
        if (!string.IsNullOrWhiteSpace(keyBase64))
            return Convert.FromBase64String(keyBase64);

        var jwtSecret = configuration["Jwt:Secret"];
        if (!string.IsNullOrWhiteSpace(jwtSecret))
            return SHA256.HashData(Encoding.UTF8.GetBytes("blind-index:" + jwtSecret));

        if (env.IsProduction())
        {
            throw new InvalidOperationException(
                "BlindIndex:KeyBase64 or Jwt:Secret is required in Production (DATA-25).");
        }

        return Encoding.UTF8.GetBytes("qimerp-dev-blind-index-key-32b!");
    }
}
