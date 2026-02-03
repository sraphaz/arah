using Araponga.Application.Interfaces;
using Araponga.Domain.Users;

namespace Araponga.Infrastructure.Shared.InMemory;

/// <summary>Implementação InMemory de IUserRepository usando InMemorySharedStore.</summary>
public sealed class InMemoryUserRepository : IUserRepository
{
    private readonly InMemorySharedStore _store;

    public InMemoryUserRepository(InMemorySharedStore store) => _store = store;

    public Task<User?> GetByAuthProviderAsync(string authProvider, string externalId, CancellationToken cancellationToken)
        => Task.FromResult(_store.Users.FirstOrDefault(u =>
            string.Equals(u.AuthProvider, authProvider, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(u.ExternalId, externalId, StringComparison.OrdinalIgnoreCase)));

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => Task.FromResult(_store.Users.FirstOrDefault(u => u.Id == id));

    public Task AddAsync(User user, CancellationToken cancellationToken)
    {
        _store.Users.Add(user);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(User user, CancellationToken cancellationToken)
    {
        var i = _store.Users.FindIndex(u => u.Id == user.Id);
        if (i >= 0) _store.Users[i] = user;
        else _store.Users.Add(user);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<User>> ListByIdsAsync(IReadOnlyCollection<Guid> userIds, CancellationToken cancellationToken)
    {
        if (userIds.Count == 0) return Task.FromResult<IReadOnlyList<User>>(Array.Empty<User>());
        return Task.FromResult<IReadOnlyList<User>>(_store.Users.Where(u => userIds.Contains(u.Id)).ToList());
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(email)) return Task.FromResult<User?>(null);
        return Task.FromResult(_store.Users.FirstOrDefault(u =>
            !string.IsNullOrWhiteSpace(u.Email) && string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase)));
    }

    public Task<IReadOnlyList<User>> SearchByDisplayNameAsync(string? query, IReadOnlyCollection<Guid>? restrictToUserIds, int limit, CancellationToken cancellationToken)
    {
        var source = restrictToUserIds is { Count: > 0 }
            ? _store.Users.Where(u => restrictToUserIds.Contains(u.Id))
            : _store.Users.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var q = query.Trim();
            source = source.Where(u => u.DisplayName.Contains(q, StringComparison.OrdinalIgnoreCase));
        }

        var list = source.OrderBy(u => u.DisplayName).Take(limit).ToList();
        return Task.FromResult<IReadOnlyList<User>>(list);
    }
}
