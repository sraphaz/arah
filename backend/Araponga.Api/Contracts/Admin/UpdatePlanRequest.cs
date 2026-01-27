namespace Araponga.Api.Contracts.Admin;

public sealed class UpdatePlanRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? PricePerCycle { get; set; }
    public string? BillingCycle { get; set; }
}
