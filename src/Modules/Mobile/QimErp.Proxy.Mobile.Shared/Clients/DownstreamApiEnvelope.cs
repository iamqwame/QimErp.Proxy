namespace QimErp.Proxy.Mobile.Shared.Clients;

public sealed class DownstreamApiEnvelope<T>
{
    public T? Data { get; set; }
    public bool IsSuccess { get; set; }
    public bool IsFailure { get; set; }
    public DownstreamApiError? Error { get; set; }
    public string? Message { get; set; }
    public string? Code { get; set; }
}

public sealed class DownstreamApiError
{
    public string? Code { get; set; }
    public string? Message { get; set; }
}
