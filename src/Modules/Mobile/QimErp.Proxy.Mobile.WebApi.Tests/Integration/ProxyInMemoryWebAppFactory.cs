using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using QimErp.Proxy.Mobile.Shared.Database;
using QimErp.Shared.Common.Interceptors;

namespace QimErp.Proxy.Mobile.WebApi.Tests.Integration;

public sealed class ProxyInMemoryWebAppFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = Guid.NewGuid().ToString();

    static ProxyInMemoryWebAppFactory()
    {
        Environment.SetEnvironmentVariable(
            "ConnectionStrings__ProxyCommands",
            "Host=localhost;Port=5432;Database=qimerp-proxy-commands-test;Username=postgres;Password=postgres;SslMode=Disable");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.UseEnvironment("Test");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["RedisCache:Enabled"] = "false",
                ["RedisCache:ConnectionString"] = "in-memory",
                ["Outbox:RelayEnabled"] = "false",
                ["ProxyCommandProtection:Key"] = Convert.ToBase64String(new byte[32]),
            });
        });

        builder.ConfigureTestServices(services =>
        {
            ReplaceDbContext(services);
            ReplaceDistributedCache(services);
        });
    }

    private void ReplaceDbContext(IServiceCollection services)
    {
        var descriptorsToRemove = services
            .Where(d =>
                (d.ServiceType.FullName?.StartsWith("Microsoft.EntityFrameworkCore.", StringComparison.Ordinal) ?? false) ||
                (d.ServiceType.FullName?.Contains("Npgsql", StringComparison.Ordinal) ?? false) ||
                (d.ImplementationType?.FullName?.Contains("Npgsql", StringComparison.Ordinal) ?? false) ||
                d.ServiceType == typeof(DbContextOptions<ProxyCommandDbContext>) ||
                d.ServiceType == typeof(ProxyCommandDbContext))
            .ToList();

        foreach (var descriptor in descriptorsToRemove)
            services.Remove(descriptor);

        services.TryAddScoped<AuditEntitySaveChangesInterceptor>();
        services.AddDbContext<ProxyCommandDbContext>((provider, options) =>
        {
            options.UseInMemoryDatabase(_databaseName);
            options.EnableSensitiveDataLogging(false);
            options.EnableDetailedErrors(false);
            options.ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            options.AddInterceptors(provider.GetRequiredService<AuditEntitySaveChangesInterceptor>());
            options.ReplaceService<IModelCustomizer, ProxyJsonColumnModelCustomizer>();
        });
    }

    private static void ReplaceDistributedCache(IServiceCollection services)
    {
        services.RemoveAll<IDistributedCache>();
        services.AddSingleton<IMemoryCache, MemoryCache>();
        services.AddSingleton<IDistributedCache>(_ =>
            new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions())));
    }
}

public sealed class ProxyJsonColumnModelCustomizer(ModelCustomizerDependencies dependencies) : ModelCustomizer(dependencies)
{
    public override void Customize(ModelBuilder modelBuilder, DbContext context)
    {
        base.Customize(modelBuilder, context);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes().ToList())
        {
            var customFieldsClrProperty = entityType.ClrType.GetProperty("CustomFields");
            if (customFieldsClrProperty is not null
                && customFieldsClrProperty.PropertyType == typeof(Dictionary<string, string>)
                && entityType.FindProperty("CustomFields")?.GetValueConverter() is null)
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property(typeof(Dictionary<string, string>), "CustomFields")
                    .HasConversion(CreateJsonConverter(typeof(Dictionary<string, string>)));
            }

            foreach (var property in entityType.GetProperties().ToList())
            {
                if (property.GetValueConverter() is not null) continue;
                if (!string.Equals(property.GetColumnType(), "jsonb", StringComparison.OrdinalIgnoreCase)) continue;

                var converter = CreateJsonConverter(property.ClrType);
                modelBuilder.Entity(entityType.ClrType).Property(property.Name).HasConversion(converter);
            }
        }
    }

    private static ValueConverter CreateJsonConverter(Type clrType)
    {
        var toString = typeof(ProxyJsonColumnModelCustomizer)
            .GetMethod(nameof(MakeConverter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .MakeGenericMethod(clrType);
        return (ValueConverter)toString.Invoke(null, null)!;
    }

    private static ValueConverter<T, string?> MakeConverter<T>()
        => new(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v => v == null ? default! : JsonSerializer.Deserialize<T>(v, (JsonSerializerOptions?)null)!);
}
