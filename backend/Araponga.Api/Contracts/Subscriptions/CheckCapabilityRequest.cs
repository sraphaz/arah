namespace Araponga.Api.Contracts.Subscriptions;

public sealed class CheckCapabilityRequest
{
    public string Capability { get; set; } = string.Empty;
    public Guid? TerritoryId { get; set; }
}
