var title = "QimERP Proxy Mobile API";
var version = "v1";
var description = "QimErp.Proxy.Mobile edge host for the Flutter ESS app. JWT-forwards to domain WebApis; no database.";

var builder = WebApplication.CreateBuilder(args);
builder.Host.AddQFaceLogging();
builder.AddQimErpOpenTelemetryDefaults();

var configuration = builder.Configuration;
var assembly = Assembly.GetExecutingAssembly();

builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();
builder.Services.AddServiceDiscovery();
builder.Services.ConfigureHttpClientDefaults(http =>
{
    http.AddStandardResilienceHandler(options =>
    {
        // Login can take 10-20s+ downstream; the default 10s attempt / 30s
        // total-request timeouts abort it. Align with Downstream:TimeoutSeconds.
        var timeout = TimeSpan.FromSeconds(configuration.GetValue<int>("Downstream:TimeoutSeconds", 90));
        options.AttemptTimeout.Timeout = timeout;
        options.TotalRequestTimeout.Timeout = timeout;
        // Circuit breaker sampling must be >= 2x attempt timeout to validate.
        options.CircuitBreaker.SamplingDuration = timeout * 2;
    });
    http.AddServiceDiscovery();
});

builder.Services.AddSwaggerDocumentation(assembly, title, version, description);
builder.Services.AddProxyMobileShared(configuration);
builder.Services.AddCoreServices(configuration, assembly);

// IAM tokens carry no employeeId claim; resolve the current employee once per
// request from the People module instead of trusting the token.
builder.Services.AddScoped<ICurrentEmployeeResolver, CurrentEmployeeResolver>();

// The Proxy forwards JWTs downstream and never runs workflow, tenant-activity,
// notification, actor, or Temporal pipelines itself. AddCoreServices registers
// those processors anyway (they drag in Temporal + actor-system dependencies);
// drop them so DI validation only covers what this host actually uses. No Proxy
// feature handler references these service types.
var proxyUnusedServices = builder.Services
    .Where(d => d.ServiceType.Namespace is { } ns &&
                (ns.StartsWith("QimErp.Shared.Common.Services.Workflow", StringComparison.Ordinal) ||
                 ns.StartsWith("QimErp.Shared.Common.Services.TenantActivity", StringComparison.Ordinal) ||
                 ns.StartsWith("QimErp.Shared.Common.Services.Notifications", StringComparison.Ordinal) ||
                 ns.StartsWith("QimErp.Shared.Common.Actors", StringComparison.Ordinal) ||
                 ns.StartsWith("QFace.Sdk.ActorSystems", StringComparison.Ordinal) ||
                 ns.StartsWith("QFace.Sdk.Temporal", StringComparison.Ordinal)))
    .ToList();
foreach (var descriptor in proxyUnusedServices)
{
    builder.Services.Remove(descriptor);
}

var app = builder.Build();

app.UseSwaggerDocumentation(title, version);
app.UseSerilogRequestLogging();
app.UseAppSecurity(configuration);

// AddCoreServices + UseAppSecurity register liveness/readiness/redis checks and
// map /health + /ready, so no extra health-check registration is needed here.
//
// Map only the Proxy's own Carter modules. Carter's global MapCarter() would
// also scan shared modules (e.g. QimErp.Shared.Common.Features.EntityCodes),
// which the Proxy — an edge host that doesn't register IEntityCodeService —
// cannot bind. Every Proxy endpoint is an ICarterModule in this assembly.
var mobileGroup = app.MapGroup("").RequireAuthorization();
foreach (var moduleType in assembly.GetTypes()
             .Where(t => !t.IsAbstract && !t.IsInterface && typeof(ICarterModule).IsAssignableFrom(t)))
{
    var module = (ICarterModule)Activator.CreateInstance(moduleType)!;
    module.AddRoutes(mobileGroup);
}

app.Run();

namespace QimErp.Proxy.Mobile.WebApi
{
    public partial class Program;
}
