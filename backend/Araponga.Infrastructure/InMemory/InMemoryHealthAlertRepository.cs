using Araponga.Application.Interfaces;
using Araponga.Domain.Health;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryHealthAlertRepository : IHealthAlertRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryHealthAlertRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<IReadOnlyList<HealthAlert>> ListByTerritoryAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        var alerts = _dataStore.HealthAlerts
            .Where(alert => alert.TerritoryId == territoryId)
            .ToList();

        return Task.FromResult<IReadOnlyList<HealthAlert>>(alerts);
    }

    public Task<HealthAlert?> GetByIdAsync(Guid alertId, CancellationToken cancellationToken)
    {
        var alert = _dataStore.HealthAlerts.FirstOrDefault(a => a.Id == alertId);
        return Task.FromResult(alert);
    }

    public Task AddAsync(HealthAlert alert, CancellationToken cancellationToken)
    {
        _dataStore.HealthAlerts.Add(alert);
        return Task.CompletedTask;
    }

    public Task UpdateStatusAsync(Guid alertId, HealthAlertStatus status, CancellationToken cancellationToken)
    {
        var alert = _dataStore.HealthAlerts.FirstOrDefault(a => a.Id == alertId);
        if (alert is null)
        {
            return Task.CompletedTask;
        }

        alert.UpdateStatus(status);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<HealthAlert>> ListByTerritoryPagedAsync(
        Guid territoryId,
        int skip,
        int take,
        CancellationToken cancellationToken)
    {
        var alerts = _dataStore.HealthAlerts
            .Where(alert => alert.TerritoryId == territoryId)
            .OrderByDescending(alert => alert.CreatedAtUtc)
            .Skip(skip)
            .Take(take)
            .ToList();

        return Task.FromResult<IReadOnlyList<HealthAlert>>(alerts);
    }

    public Task<int> CountByTerritoryAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        const int maxInt32 = int.MaxValue;
        var count = _dataStore.HealthAlerts.Count(alert => alert.TerritoryId == territoryId);
        return Task.FromResult(count > maxInt32 ? maxInt32 : count);
    }
}
