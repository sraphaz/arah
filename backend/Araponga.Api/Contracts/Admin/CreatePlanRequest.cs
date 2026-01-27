namespace Araponga.Api.Contracts.Admin;

public sealed class CreatePlanRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Tier { get; set; } = string.Empty;
    public decimal? PricePerCycle { get; set; }
    public string? BillingCycle { get; set; }
    public List<string> Capabilities { get; set; } = new();
    public Dictionary<string, object>? Limits { get; set; }
    public int? TrialDays { get; set; }
}
