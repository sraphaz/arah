namespace Araponga.Api.Contracts.Subscriptions;

public sealed class CancelSubscriptionRequest
{
    public bool CancelAtPeriodEnd { get; set; } = true;
}
