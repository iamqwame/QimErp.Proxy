using System.Text.Json;

namespace QimErp.Proxy.Mobile.Shared.Contracts;

public sealed class MobileHomeResponse
{
    public JsonElement? EssHome { get; set; }
    public JsonElement? LeaveBalance { get; set; }
    public JsonElement? PendingApprovals { get; set; }
    public JsonElement? PayrollSummary { get; set; }
    public IReadOnlyList<string> DegradedSections { get; set; } = [];
}

public sealed class MobileMeResponse
{
    public JsonElement? User { get; set; }
    public JsonElement? Employee { get; set; }
    public IReadOnlyList<string> DegradedSections { get; set; } = [];
}
