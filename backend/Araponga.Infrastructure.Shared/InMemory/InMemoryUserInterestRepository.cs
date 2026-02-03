using Araponga.Application.Interfaces;
using Araponga.Domain.Users;

namespace Araponga.Infrastructure.Shared.InMemory;

/// <summary>Implementação InMemory de IUserInterestRepository usando InMemorySharedStore.</summary>
public sealed class InMemoryUserInterestRepository : IUserInterestRepository
{
    private readonly InMemorySharedStore _store;

    public InMemoryUserInterestRepository(InMemorySharedStore store) => _store = store;

    public Task AddAsync(UserInterest interest, CancellationToken cancellationToken)
    {
        _store.UserInterests.Add(interest);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(Guid userId, string interestTag, CancellationToken cancellationToken)
    {
        var tag = interestTag.Trim().ToLowerInvariant();
        var i = _store.UserInterests.FirstOrDefault(x => x.UserId == userId && x.InterestTag == tag);
        if (i is not null) _store.UserInterests.Remove(i);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<UserInterest>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        => Task.FromResult<IReadOnlyList<UserInterest>>(_store.UserInterests.Where(i => i.UserId == userId).OrderBy(i => i.InterestTag).ToList());

    public Task<IReadOnlyList<Guid>> ListUserIdsByInterestAsync(string interestTag, Guid territoryId, CancellationToken cancellationToken)
    {
        var tag = interestTag.Trim().ToLowerInvariant();
        var userIds = _store.UserInterests
            .Where(i => i.InterestTag == tag)
            .Select(i => i.UserId)
            .Where(uid => _store.Memberships.Any(m => m.UserId == uid && m.TerritoryId == territoryId))
            .Distinct()
            .ToList();
        return Task.FromResult<IReadOnlyList<Guid>>(userIds);
    }

    public Task<bool> ExistsAsync(Guid userId, string interestTag, CancellationToken cancellationToken)
    {
        var tag = interestTag.Trim().ToLowerInvariant();
        return Task.FromResult(_store.UserInterests.Any(i => i.UserId == userId && i.InterestTag == tag));
    }

    public Task<int> CountByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        => Task.FromResult(_store.UserInterests.Count(i => i.UserId == userId));
}
