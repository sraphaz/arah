using Araponga.Application.Interfaces;
using Araponga.Domain.Users;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryUserRepository : IUserRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryUserRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<User?> GetByAuthProviderAsync(string authProvider, string externalId, CancellationToken cancellationToken)
    {
        var user = _dataStore.Users.FirstOrDefault(u =>
            string.Equals(u.AuthProvider, authProvider, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(u.ExternalId, externalId, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(user);
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = _dataStore.Users.FirstOrDefault(u => u.Id == id);
        return Task.FromResult(user);
    }

    public Task AddAsync(User user, CancellationToken cancellationToken)
    {
        _dataStore.Users.Add(user);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(User user, CancellationToken cancellationToken)
    {
        var index = _dataStore.Users.FindIndex(u => u.Id == user.Id);
        if (index >= 0)
        {
            _dataStore.Users[index] = user;
        }
        else
        {
            _dataStore.Users.Add(user);
        }

        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<User>> ListByIdsAsync(
        IReadOnlyCollection<Guid> userIds,
        CancellationToken cancellationToken)
    {
        if (userIds.Count == 0)
        {
            return Task.FromResult<IReadOnlyList<User>>(Array.Empty<User>());
        }

        var users = _dataStore.Users
            .Where(user => userIds.Contains(user.Id))
            .ToList();

        return Task.FromResult<IReadOnlyList<User>>(users);
    }
}
