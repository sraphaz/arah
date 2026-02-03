using Araponga.Domain.Connections;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryConnectionPrivacySettingsRepository : IConnectionPrivacySettingsRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryConnectionPrivacySettingsRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<ConnectionPrivacySettings?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var s = _dataStore.ConnectionPrivacySettings.FirstOrDefault(x => x.UserId == userId);
        return Task.FromResult(s);
    }

    public Task<ConnectionPrivacySettings> AddAsync(ConnectionPrivacySettings settings, CancellationToken cancellationToken)
    {
        _dataStore.ConnectionPrivacySettings.Add(settings);
        return Task.FromResult(settings);
    }

    public Task UpdateAsync(ConnectionPrivacySettings settings, CancellationToken cancellationToken)
    {
        var idx = _dataStore.ConnectionPrivacySettings.FindIndex(x => x.UserId == settings.UserId);
        if (idx >= 0)
            _dataStore.ConnectionPrivacySettings[idx] = settings;
        return Task.CompletedTask;
    }
}
