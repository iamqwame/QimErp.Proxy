namespace QimErp.Proxy.Mobile.Shared.Options;

public class DownstreamOptions
{
    public const string SectionName = "Downstream";

    public string Iam { get; set; } = string.Empty;
    public string People { get; set; } = string.Empty;
    public string Leave { get; set; } = string.Empty;
    public string Payroll { get; set; } = string.Empty;
    public string Performance { get; set; } = string.Empty;
    public string Workflow { get; set; } = string.Empty;
    public string Benefit { get; set; } = string.Empty;
    public string Surveys { get; set; } = string.Empty;
    public string Notifications { get; set; } = string.Empty;

    /// <summary>Downstream HttpClient timeout (login can take 10-20s+ in dev).</summary>
    public int TimeoutSeconds { get; set; } = 90;
}
