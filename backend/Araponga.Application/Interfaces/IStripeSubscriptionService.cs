using Araponga.Application.Common;
using Araponga.Domain.Subscriptions;

namespace Araponga.Application.Interfaces;

public interface IStripeSubscriptionService
{
    Task<OperationResult<StripeSubscriptionResult>> CreateSubscriptionAsync(
        Guid userId,
        Guid planId,
        string? couponCode,
        CancellationToken cancellationToken);
    
    Task<OperationResult<StripeSubscriptionResult>> UpdateSubscriptionAsync(
        Guid subscriptionId,
        Guid newPlanId,
        CancellationToken cancellationToken);
    
    Task<OperationResult> CancelSubscriptionAsync(
        Guid subscriptionId,
        bool cancelAtPeriodEnd,
        CancellationToken cancellationToken);
    
    Task<OperationResult> ReactivateSubscriptionAsync(
        Guid subscriptionId,
        CancellationToken cancellationToken);
    
    Task<StripeSubscriptionInfo?> GetSubscriptionAsync(
        string stripeSubscriptionId,
        CancellationToken cancellationToken);
}

public sealed class StripeSubscriptionResult
{
    public string StripeSubscriptionId { get; init; } = string.Empty;
    public string StripeCustomerId { get; init; } = string.Empty;
    public DateTime CurrentPeriodStart { get; init; }
    public DateTime CurrentPeriodEnd { get; init; }
    public SubscriptionStatus Status { get; init; }
}

public sealed class StripeSubscriptionInfo
{
    public string StripeSubscriptionId { get; init; } = string.Empty;
    public string StripeCustomerId { get; init; } = string.Empty;
    public SubscriptionStatus Status { get; init; }
    public DateTime CurrentPeriodStart { get; init; }
    public DateTime CurrentPeriodEnd { get; init; }
    public DateTime? TrialEnd { get; init; }
    public bool CancelAtPeriodEnd { get; init; }
}
