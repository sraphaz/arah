namespace Arah.Domain.Financial;

/// <summary>
/// Medidor de consumo comercial (FASE55 v0) — uso vs cota por métrica.
/// </summary>
public sealed class ConsumptionMeter
{
    public ConsumptionMeter(
        Guid id,
        Guid subscriptionId,
        string metric,
        decimal usage,
        decimal quota,
        decimal overageRate)
    {
        if (subscriptionId == Guid.Empty)
        {
            throw new ArgumentException("Subscription ID is required.", nameof(subscriptionId));
        }

        if (string.IsNullOrWhiteSpace(metric))
        {
            throw new ArgumentException("Metric is required.", nameof(metric));
        }

        if (usage < 0)
        {
            throw new ArgumentException("Usage cannot be negative.", nameof(usage));
        }

        if (quota < 0)
        {
            throw new ArgumentException("Quota cannot be negative.", nameof(quota));
        }

        if (overageRate < 0)
        {
            throw new ArgumentException("Overage rate cannot be negative.", nameof(overageRate));
        }

        Id = id;
        SubscriptionId = subscriptionId;
        Metric = metric.Trim().ToLowerInvariant();
        Usage = usage;
        Quota = quota;
        OverageRate = overageRate;
    }

    public Guid Id { get; }
    public Guid SubscriptionId { get; }
    public string Metric { get; }
    public decimal Usage { get; private set; }
    public decimal Quota { get; }
    public decimal OverageRate { get; }

    public static IReadOnlyList<ConsumptionMeter> ZeroedDefaults(Guid subscriptionId) =>
        new[]
        {
            new ConsumptionMeter(Guid.NewGuid(), subscriptionId, "ai", 0m, 100m, 0m),
            new ConsumptionMeter(Guid.NewGuid(), subscriptionId, "media", 0m, 1000m, 0m),
            new ConsumptionMeter(Guid.NewGuid(), subscriptionId, "notifications", 0m, 5000m, 0m),
        };
}
