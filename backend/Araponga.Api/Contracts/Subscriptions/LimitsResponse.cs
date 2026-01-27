namespace Araponga.Api.Contracts.Subscriptions;

public sealed class LimitsResponse
{
    public Dictionary<string, object> Limits { get; set; } = new();
}
