using Araponga.Domain.Subscriptions;
using Araponga.Modules.Subscriptions.Infrastructure.Postgres.Entities;
using System.Text.Json;

namespace Araponga.Modules.Subscriptions.Infrastructure.Postgres;

internal static class SubscriptionsMappers
{
    public static SubscriptionPlanRecord ToRecord(this SubscriptionPlan plan)
    {
        return new SubscriptionPlanRecord
        {
            Id = plan.Id,
            Name = plan.Name,
            Description = plan.Description,
            Tier = (int)plan.Tier,
            Scope = (int)plan.Scope,
            TerritoryId = plan.TerritoryId,
            PricePerCycle = plan.PricePerCycle,
            BillingCycle = plan.BillingCycle.HasValue ? (int)plan.BillingCycle.Value : null,
            CapabilitiesJson = JsonSerializer.Serialize(plan.Capabilities),
            LimitsJson = plan.Limits != null ? JsonSerializer.Serialize(plan.Limits) : null,
            IsDefault = plan.IsDefault,
            TrialDays = plan.TrialDays,
            IsActive = plan.IsActive,
            CreatedByUserId = plan.CreatedByUserId,
            StripePriceId = plan.StripePriceId,
            StripeProductId = plan.StripeProductId,
            CreatedAtUtc = plan.CreatedAtUtc,
            UpdatedAtUtc = plan.UpdatedAtUtc
        };
    }

    public static SubscriptionPlan ToDomain(this SubscriptionPlanRecord record)
    {
        var capabilities = JsonSerializer.Deserialize<List<FeatureCapability>>(record.CapabilitiesJson) ?? new List<FeatureCapability>();
        Dictionary<string, object>? limits = null;
        if (!string.IsNullOrWhiteSpace(record.LimitsJson))
        {
            try
            {
                limits = JsonSerializer.Deserialize<Dictionary<string, object>>(record.LimitsJson);
            }
            catch
            {
                // Se falhar, limits permanece null
            }
        }

        return new SubscriptionPlan(
            record.Id,
            record.Name,
            record.Description,
            (SubscriptionPlanTier)record.Tier,
            (PlanScope)record.Scope,
            record.TerritoryId,
            record.PricePerCycle,
            record.BillingCycle.HasValue ? (SubscriptionBillingCycle)record.BillingCycle.Value : null,
            capabilities,
            limits,
            record.IsDefault,
            record.TrialDays,
            record.CreatedByUserId,
            record.StripePriceId,
            record.StripeProductId);
    }

    public static SubscriptionRecord ToRecord(this Subscription subscription)
    {
        return new SubscriptionRecord
        {
            Id = subscription.Id,
            UserId = subscription.UserId,
            TerritoryId = subscription.TerritoryId,
            PlanId = subscription.PlanId,
            Status = (int)subscription.Status,
            CurrentPeriodStart = subscription.CurrentPeriodStart,
            CurrentPeriodEnd = subscription.CurrentPeriodEnd,
            TrialStart = subscription.TrialStart,
            TrialEnd = subscription.TrialEnd,
            CanceledAt = subscription.CanceledAt,
            CancelAtPeriodEnd = subscription.CancelAtPeriodEnd,
            StripeSubscriptionId = subscription.StripeSubscriptionId,
            StripeCustomerId = subscription.StripeCustomerId,
            CreatedAtUtc = subscription.CreatedAtUtc,
            UpdatedAtUtc = subscription.UpdatedAtUtc
        };
    }

    public static Subscription ToDomain(this SubscriptionRecord record)
    {
        return new Subscription(
            record.Id,
            record.UserId,
            record.TerritoryId,
            record.PlanId,
            (SubscriptionStatus)record.Status,
            record.CurrentPeriodStart,
            record.CurrentPeriodEnd,
            record.TrialStart,
            record.TrialEnd,
            record.StripeSubscriptionId,
            record.StripeCustomerId);
    }

    public static SubscriptionPaymentRecord ToRecord(this SubscriptionPayment payment)
    {
        return new SubscriptionPaymentRecord
        {
            Id = payment.Id,
            SubscriptionId = payment.SubscriptionId,
            Amount = payment.Amount,
            Currency = payment.Currency,
            Status = (int)payment.Status,
            PaymentDate = payment.PaymentDate,
            PeriodStart = payment.PeriodStart,
            PeriodEnd = payment.PeriodEnd,
            StripeInvoiceId = payment.StripeInvoiceId,
            StripePaymentIntentId = payment.StripePaymentIntentId,
            FailureReason = payment.FailureReason,
            CreatedAtUtc = payment.CreatedAtUtc,
            UpdatedAtUtc = payment.UpdatedAtUtc
        };
    }

    public static SubscriptionPayment ToDomain(this SubscriptionPaymentRecord record)
    {
        return new SubscriptionPayment(
            record.Id,
            record.SubscriptionId,
            record.Amount,
            record.Currency,
            (SubscriptionPaymentStatus)record.Status,
            record.PaymentDate,
            record.PeriodStart,
            record.PeriodEnd,
            record.StripeInvoiceId,
            record.StripePaymentIntentId,
            record.FailureReason);
    }

    public static CouponRecord ToRecord(this Coupon coupon)
    {
        return new CouponRecord
        {
            Id = coupon.Id,
            Code = coupon.Code,
            Name = coupon.Name,
            Description = coupon.Description,
            DiscountType = (int)coupon.DiscountType,
            DiscountValue = coupon.DiscountValue,
            ValidFrom = coupon.ValidFrom,
            ValidUntil = coupon.ValidUntil,
            MaxUses = coupon.MaxUses,
            UsedCount = coupon.UsedCount,
            IsActive = coupon.IsActive,
            StripeCouponId = coupon.StripeCouponId,
            CreatedAtUtc = coupon.CreatedAtUtc,
            UpdatedAtUtc = coupon.UpdatedAtUtc
        };
    }

    public static Coupon ToDomain(this CouponRecord record)
    {
        var coupon = new Coupon(
            record.Id,
            record.Code,
            record.Name,
            record.Description,
            (CouponDiscountType)record.DiscountType,
            record.DiscountValue,
            record.ValidFrom,
            record.ValidUntil,
            record.MaxUses,
            record.StripeCouponId);

        coupon.RestoreState(record.UsedCount, record.IsActive, record.CreatedAtUtc, record.UpdatedAtUtc);

        return coupon;
    }

    public static SubscriptionCouponRecord ToRecord(this SubscriptionCoupon subscriptionCoupon)
    {
        return new SubscriptionCouponRecord
        {
            Id = subscriptionCoupon.Id,
            SubscriptionId = subscriptionCoupon.SubscriptionId,
            CouponId = subscriptionCoupon.CouponId,
            AppliedAtUtc = subscriptionCoupon.AppliedAtUtc
        };
    }

    public static SubscriptionCoupon ToDomain(this SubscriptionCouponRecord record)
    {
        return new SubscriptionCoupon(
            record.Id,
            record.SubscriptionId,
            record.CouponId);
    }

    public static SubscriptionPlanHistoryRecord ToRecord(this SubscriptionPlanHistory history)
    {
        return new SubscriptionPlanHistoryRecord
        {
            Id = history.Id,
            PlanId = history.PlanId,
            ChangedByUserId = history.ChangedByUserId,
            ChangeType = (int)history.ChangeType,
            PreviousStateJson = history.PreviousState != null ? JsonSerializer.Serialize(history.PreviousState) : null,
            NewStateJson = history.NewState != null ? JsonSerializer.Serialize(history.NewState) : null,
            ChangeReason = history.ChangeReason,
            ChangedAtUtc = history.ChangedAtUtc
        };
    }

    public static SubscriptionPlanHistory ToDomain(this SubscriptionPlanHistoryRecord record)
    {
        Dictionary<string, object>? previousState = null;
        if (!string.IsNullOrWhiteSpace(record.PreviousStateJson))
        {
            try
            {
                previousState = JsonSerializer.Deserialize<Dictionary<string, object>>(record.PreviousStateJson);
            }
            catch
            {
                // Se falhar, previousState permanece null
            }
        }

        Dictionary<string, object>? newState = null;
        if (!string.IsNullOrWhiteSpace(record.NewStateJson))
        {
            try
            {
                newState = JsonSerializer.Deserialize<Dictionary<string, object>>(record.NewStateJson);
            }
            catch
            {
                // Se falhar, newState permanece null
            }
        }

        return new SubscriptionPlanHistory(
            record.Id,
            record.PlanId,
            record.ChangedByUserId,
            (SubscriptionPlanHistoryChangeType)record.ChangeType,
            previousState,
            newState,
            record.ChangeReason);
    }
}
