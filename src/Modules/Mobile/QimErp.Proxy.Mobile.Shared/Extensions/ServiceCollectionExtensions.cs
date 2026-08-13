namespace QimErp.Proxy.Mobile.Shared.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProxyMobileShared(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DownstreamOptions>(configuration.GetSection(DownstreamOptions.SectionName));
        var options = configuration.GetSection(DownstreamOptions.SectionName).Get<DownstreamOptions>()
                      ?? new DownstreamOptions();

        RegisterClient(services, DownstreamClientNames.Iam, options.Iam, options.TimeoutSeconds);
        RegisterClient(services, DownstreamClientNames.People, options.People, options.TimeoutSeconds);
        RegisterClient(services, DownstreamClientNames.Leave, options.Leave, options.TimeoutSeconds);
        RegisterClient(services, DownstreamClientNames.Payroll, options.Payroll, options.TimeoutSeconds);
        RegisterClient(services, DownstreamClientNames.Performance, options.Performance, options.TimeoutSeconds);
        RegisterClient(services, DownstreamClientNames.Workflow, options.Workflow, options.TimeoutSeconds);
        RegisterClient(services, DownstreamClientNames.Benefit, options.Benefit, options.TimeoutSeconds);
        RegisterClient(services, DownstreamClientNames.Surveys, options.Surveys, options.TimeoutSeconds);
        RegisterClient(services, DownstreamClientNames.Notifications, options.Notifications, options.TimeoutSeconds);

        services.AddScoped<IIamDownstreamClient, IamDownstreamClient>();
        services.AddScoped<IPeopleDownstreamClient, PeopleDownstreamClient>();
        services.AddScoped<ILeaveDownstreamClient, LeaveDownstreamClient>();
        services.AddScoped<IPayrollDownstreamClient, PayrollDownstreamClient>();
        services.AddScoped<IPerformanceDownstreamClient, PerformanceDownstreamClient>();
        services.AddScoped<IWorkflowDownstreamClient, WorkflowDownstreamClient>();
        services.AddScoped<IBenefitDownstreamClient, BenefitDownstreamClient>();
        services.AddScoped<ISurveysDownstreamClient, SurveysDownstreamClient>();
        services.AddScoped<INotificationsDownstreamClient, NotificationsDownstreamClient>();

        return services;
    }

    private static void RegisterClient(IServiceCollection services, string name, string baseUrl, int timeoutSeconds)
    {
        services.AddHttpClient(name, client =>
        {
            if (!string.IsNullOrWhiteSpace(baseUrl))
            {
                client.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");
            }

            client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
        });
    }
}
