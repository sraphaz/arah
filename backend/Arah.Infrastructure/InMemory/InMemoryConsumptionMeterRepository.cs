using System.Collections.Concurrent;
using Arah.Application.Interfaces;
using Arah.Domain.Financial;

namespace Arah.Infrastructure.InMemory;

public sealed class InMemoryConsumptionMeterRepository : IConsumptionMeterRepository
{
    private readonly ConcurrentDictionary<Guid, List<ConsumptionMeter>> _bySubscription = new();

    public Task<IReadOnlyList<ConsumptionMeter>> ListBySubscriptionAsync(
        Guid subscriptionId,
        CancellationToken cancellationToken)
    {
        if (_bySubscription.TryGetValue(subscriptionId, out var list))
        {
            lock (list)
            {
                return Task.FromResult<IReadOnlyList<ConsumptionMeter>>(list.ToList());
            }
        }

        return Task.FromResult<IReadOnlyList<ConsumptionMeter>>(Array.Empty<ConsumptionMeter>());
    }

    public Task AddRangeAsync(IEnumerable<ConsumptionMeter> meters, CancellationToken cancellationToken)
    {
        foreach (var group in meters.GroupBy(m => m.SubscriptionId))
        {
            var list = _bySubscription.GetOrAdd(group.Key, _ => new List<ConsumptionMeter>());
            lock (list)
            {
                foreach (var meter in group)
                {
                    if (list.Any(existing =>
                            string.Equals(existing.Metric, meter.Metric, StringComparison.Ordinal)))
                    {
                        continue;
                    }

                    list.Add(meter);
                }
            }
        }

        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<ConsumptionMeter>> GetOrCreateDefaultsAsync(
        Guid subscriptionId,
        CancellationToken cancellationToken)
    {
        var list = _bySubscription.GetOrAdd(subscriptionId, _ => new List<ConsumptionMeter>());
        lock (list)
        {
            if (list.Count == 0)
            {
                list.AddRange(ConsumptionMeter.ZeroedDefaults(subscriptionId));
            }

            return Task.FromResult<IReadOnlyList<ConsumptionMeter>>(list.ToList());
        }
    }
}
