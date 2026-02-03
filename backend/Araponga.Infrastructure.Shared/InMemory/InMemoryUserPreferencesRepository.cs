using Araponga.Application.Interfaces;
using Araponga.Domain.Users;

namespace Araponga.Infrastructure.Shared.InMemory;

/// <summary>Implementação InMemory de IUserPreferencesRepository usando InMemorySharedStore.</summary>
public sealed class InMemoryUserPreferencesRepository : IUserPreferencesRepository
{
    private readonly InMemorySharedStore _store;

    public InMemoryUserPreferencesRepository(InMemorySharedStore store) => _store = store;

    public Task<UserPreferences?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        => Task.FromResult(_store.UserPreferences.FirstOrDefault(p => p.UserId == userId));

    public async Task<UserPreferences> GetOrCreateDefaultAsync(Guid userId, CancellationToken cancellationToken)
    {
        var existing = await GetByUserIdAsync(userId, cancellationToken);
        if (existing is not null) return existing;
        var def = UserPreferences.CreateDefault(userId, DateTime.UtcNow);
        await AddAsync(def, cancellationToken);
        return def;
    }

    public Task AddAsync(UserPreferences preferences, CancellationToken cancellationToken)
    {
        _store.UserPreferences.Add(preferences);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(UserPreferences preferences, CancellationToken cancellationToken)
    {
        var existing = _store.UserPreferences.FirstOrDefault(p => p.UserId == preferences.UserId);
        if (existing is not null) _store.UserPreferences.Remove(existing);
        _store.UserPreferences.Add(preferences);
        return Task.CompletedTask;
    }
}
