namespace QimErp.Proxy.Mobile.Shared.Clients;

public interface ILeaveDownstreamClient
{
    Task<Result<JsonElement>> GetBalanceAsync(CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> GetHistoryAsync(CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> GetMyRequestsAsync(CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> GetConfiguredAsync(CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> CalculateAsync(string queryString, CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> GetLeaveTypesAsync(CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> GetPlannerAsync(string scope, CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> GetHolidaysAsync(int? year, CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> GetTravelPermissionsAsync(object body, CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> RequestAsync(object body, CancellationToken cancellationToken = default);
    Task<Result<JsonElement>> CreateTravelPermissionAsync(object body, CancellationToken cancellationToken = default);
}

public sealed class LeaveDownstreamClient(
    IHttpClientFactory httpClientFactory,
    IHttpContextAccessor httpContextAccessor,
    ILogger<LeaveDownstreamClient> logger)
    : DownstreamHttpClientBase(httpClientFactory, httpContextAccessor, logger), ILeaveDownstreamClient
{
    protected override string ClientName => DownstreamClientNames.Leave;

    public Task<Result<JsonElement>> GetBalanceAsync(CancellationToken cancellationToken = default)
        => GetRawAsync(MobileApiConstants.Downstream.LeaveBalance, cancellationToken);

    public Task<Result<JsonElement>> GetHistoryAsync(CancellationToken cancellationToken = default)
        => GetRawAsync(MobileApiConstants.Downstream.LeaveHistory, cancellationToken);

    public Task<Result<JsonElement>> GetMyRequestsAsync(CancellationToken cancellationToken = default)
        => GetRawAsync(MobileApiConstants.Downstream.LeaveMyRequests, cancellationToken);

    public Task<Result<JsonElement>> GetConfiguredAsync(CancellationToken cancellationToken = default)
        => GetRawAsync(MobileApiConstants.Downstream.LeaveConfigured, cancellationToken);

    public Task<Result<JsonElement>> CalculateAsync(string queryString, CancellationToken cancellationToken = default)
        => GetRawAsync($"{MobileApiConstants.Downstream.LeaveCalculate}{queryString}", cancellationToken);

    public Task<Result<JsonElement>> GetLeaveTypesAsync(CancellationToken cancellationToken = default)
        => GetRawAsync(MobileApiConstants.Downstream.LeaveTypes, cancellationToken);

    public Task<Result<JsonElement>> GetPlannerAsync(string scope, CancellationToken cancellationToken = default)
        => GetRawAsync($"{MobileApiConstants.Downstream.LeavePlanner}?scope={scope}", cancellationToken);

    public Task<Result<JsonElement>> GetHolidaysAsync(int? year, CancellationToken cancellationToken = default)
        => GetRawAsync(year.HasValue
            ? $"{MobileApiConstants.Downstream.LeaveHolidays}?year={year}"
            : MobileApiConstants.Downstream.LeaveHolidays, cancellationToken);

    public Task<Result<JsonElement>> GetTravelPermissionsAsync(object body, CancellationToken cancellationToken = default)
        => PostRawAsync(MobileApiConstants.Downstream.LeaveTravelMyPage, body, cancellationToken);

    public Task<Result<JsonElement>> RequestAsync(object body, CancellationToken cancellationToken = default)
        => PostFormRawAsync(
            MobileApiConstants.Downstream.LeaveRequest,
            FlattenBody(body),
            cancellationToken);

    // The travel-permission endpoint binds from multipart form (it accepts file
    // attachments), so flatten the JSON body into form fields like leave requests.
    public Task<Result<JsonElement>> CreateTravelPermissionAsync(object body, CancellationToken cancellationToken = default)
        => PostFormRawAsync(
            MobileApiConstants.Downstream.LeaveTravelCreate,
            FlattenBody(body),
            cancellationToken);

    // The leave endpoint binds from multipart form (it accepts file attachments),
    // so flatten the incoming JSON body into form fields.
    private static IReadOnlyDictionary<string, object?> FlattenBody(object body)
    {
        var json = JsonSerializer.Serialize(body);
        using var doc = JsonDocument.Parse(json);
        var fields = new Dictionary<string, object?>();
        foreach (var prop in doc.RootElement.EnumerateObject())
        {
            var value = prop.Value;
            fields[prop.Name] = value.ValueKind switch
            {
                JsonValueKind.Null => null,
                JsonValueKind.String => value.GetString(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Number when value.TryGetInt64(out var l) => l,
                JsonValueKind.Number => value.GetDouble(),
                _ => value.GetRawText(),
            };
        }
        return fields;
    }
}
