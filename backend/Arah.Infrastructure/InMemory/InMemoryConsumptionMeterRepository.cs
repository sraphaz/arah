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
            return Task.FromResult<IReadOnlyList<ConsumptionMeter>>(list.ToList());
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
                list.AddRange(group);
            }
        }

        return Task.CompletedTask;
    }
}
