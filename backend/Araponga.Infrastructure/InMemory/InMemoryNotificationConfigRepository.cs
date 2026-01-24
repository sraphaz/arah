using Araponga.Application.Interfaces.Notifications;
using Araponga.Domain.Notifications;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryNotificationConfigRepository : INotificationConfigRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryNotificationConfigRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<NotificationConfig?> GetByTerritoryIdAsync(Guid? territoryId, CancellationToken cancellationToken)
    {
        var config = _dataStore.NotificationConfigs
            .FirstOrDefault(c => c.TerritoryId == territoryId);

        return Task.FromResult(config);
    }

    public Task<NotificationConfig?> GetGlobalAsync(CancellationToken cancellationToken)
    {
        return GetByTerritoryIdAsync(null, cancellationToken);
    }

    public Task SaveAsync(NotificationConfig config, CancellationToken cancellationToken)
    {
        var existing = _dataStore.NotificationConfigs
            .FirstOrDefault(c => c.Id == config.Id);

        if (existing is null)
        {
            _dataStore.NotificationConfigs.Add(config);
        }
        else
        {
            var index = _dataStore.NotificationConfigs.IndexOf(existing);
            _dataStore.NotificationConfigs[index] = config;
        }

        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<NotificationConfig>> ListTerritorialAsync(CancellationToken cancellationToken)
    {
        var configs = _dataStore.NotificationConfigs
            .Where(c => c.TerritoryId != null)
            .ToList();

        return Task.FromResult<IReadOnlyList<NotificationConfig>>(configs);
    }
}
