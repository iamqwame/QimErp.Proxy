using System.Text.Json;
using QimErp.Proxy.Mobile.Shared.Clients;

namespace QimErp.Proxy.Mobile.WebApi.Services;

/// <summary>
/// Resolves the CoreHR employee GUID for the signed-in user. The IAM login token
/// carries no employeeId claim, so the id is looked up from the People module
/// (/hr/employees/me, resolved by the token email) once per request and cached.
/// </summary>
public interface ICurrentEmployeeResolver
{
    Task<Guid?> ResolveAsync(CancellationToken cancellationToken = default);
}

public sealed class CurrentEmployeeResolver(IPeopleDownstreamClient peopleClient)
    : ICurrentEmployeeResolver
{
    private Guid? _cached;

    public async Task<Guid?> ResolveAsync(CancellationToken cancellationToken = default)
    {
        if (_cached is not null)
            return _cached;

        var result = await peopleClient.GetCurrentEmployeeAsync(cancellationToken);
        if (result.IsSuccess &&
            result.Data.TryGetProperty("id", out var idProperty) &&
            Guid.TryParse(idProperty.GetString(), out var id))
        {
            _cached = id;
            return id;
        }

        return null;
    }
}
