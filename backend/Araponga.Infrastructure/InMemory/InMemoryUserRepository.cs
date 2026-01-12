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

    public Task<User?> GetByProviderAsync(string provider, string externalId, CancellationToken cancellationToken)
    {
        var user = _dataStore.Users.FirstOrDefault(u =>
            string.Equals(u.Provider, provider, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(u.ExternalId, externalId, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(user);
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = _dataStore.Users.FirstOrDefault(u => u.Id == id);
        return Task.FromResult(user);
    }

    public Task<IReadOnlyList<Guid>> ListUserIdsByRoleAsync(UserRole role, CancellationToken cancellationToken)
    {
        var userIds = _dataStore.Users
            .Where(user => user.Role == role)
            .Select(user => user.Id)
            .ToList();

        return Task.FromResult<IReadOnlyList<Guid>>(userIds);
    }

    public Task AddAsync(User user, CancellationToken cancellationToken)
    {
        _dataStore.Users.Add(user);
        return Task.CompletedTask;
    }
}
