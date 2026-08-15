using Arah.Domain.Financial;

namespace Arah.Application.Interfaces;

public interface IConsumptionMeterRepository
{
    Task<IReadOnlyList<ConsumptionMeter>> ListBySubscriptionAsync(
        Guid subscriptionId,
        CancellationToken cancellationToken);

    Task AddRangeAsync(IEnumerable<ConsumptionMeter> meters, CancellationToken cancellationToken);

    /// <summary>
    /// Atomically returns existing meters or seeds zeroed defaults once per subscription.
    /// </summary>
    Task<IReadOnlyList<ConsumptionMeter>> GetOrCreateDefaultsAsync(
        Guid subscriptionId,
        CancellationToken cancellationToken);
}
