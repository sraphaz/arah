using Araponga.Application.Interfaces;
using Araponga.Domain.Users;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryUserPreferencesRepository : IUserPreferencesRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryUserPreferencesRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<UserPreferences?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var preferences = _dataStore.UserPreferences.FirstOrDefault(p => p.UserId == userId);
        return Task.FromResult(preferences);
    }

    public async Task<UserPreferences> GetOrCreateDefaultAsync(Guid userId, CancellationToken cancellationToken)
    {
        var existing = await GetByUserIdAsync(userId, cancellationToken);
        if (existing is not null)
        {
            return existing;
        }

        var defaultPreferences = UserPreferences.CreateDefault(userId, DateTime.UtcNow);
        await AddAsync(defaultPreferences, cancellationToken);
        return defaultPreferences;
    }

    public Task AddAsync(UserPreferences preferences, CancellationToken cancellationToken)
    {
        _dataStore.UserPreferences.Add(preferences);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(UserPreferences preferences, CancellationToken cancellationToken)
    {
        // InMemory: a referência já está na lista, então não precisa fazer nada
        // Mas vamos garantir que existe
        var existing = _dataStore.UserPreferences.FirstOrDefault(p => p.UserId == preferences.UserId);
        if (existing is null)
        {
            _dataStore.UserPreferences.Add(preferences);
        }

        return Task.CompletedTask;
    }
}
