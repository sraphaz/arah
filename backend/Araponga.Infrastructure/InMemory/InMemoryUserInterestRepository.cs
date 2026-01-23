using Araponga.Application.Interfaces;
using Araponga.Domain.Users;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryUserInterestRepository : IUserInterestRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryUserInterestRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task AddAsync(UserInterest interest, CancellationToken cancellationToken)
    {
        _dataStore.UserInterests.Add(interest);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(Guid userId, string interestTag, CancellationToken cancellationToken)
    {
        var normalizedTag = interestTag.Trim().ToLowerInvariant();
        var interest = _dataStore.UserInterests
            .FirstOrDefault(i => i.UserId == userId && i.InterestTag == normalizedTag);

        if (interest is not null)
        {
            _dataStore.UserInterests.Remove(interest);
        }

        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<UserInterest>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var interests = _dataStore.UserInterests
            .Where(i => i.UserId == userId)
            .OrderBy(i => i.InterestTag)
            .ToList();

        return Task.FromResult<IReadOnlyList<UserInterest>>(interests);
    }

    public Task<IReadOnlyList<Guid>> ListUserIdsByInterestAsync(string interestTag, Guid territoryId, CancellationToken cancellationToken)
    {
        var normalizedTag = interestTag.Trim().ToLowerInvariant();

        // Buscar usuários que têm o interesse E são membros do território
        var userIds = _dataStore.UserInterests
            .Where(i => i.InterestTag == normalizedTag)
            .Select(i => i.UserId)
            .Where(userId => _dataStore.Memberships
                .Any(m => m.UserId == userId && m.TerritoryId == territoryId))
            .Distinct()
            .ToList();

        return Task.FromResult<IReadOnlyList<Guid>>(userIds);
    }

    public Task<bool> ExistsAsync(Guid userId, string interestTag, CancellationToken cancellationToken)
    {
        var normalizedTag = interestTag.Trim().ToLowerInvariant();
        var exists = _dataStore.UserInterests
            .Any(i => i.UserId == userId && i.InterestTag == normalizedTag);

        return Task.FromResult(exists);
    }

    public Task<int> CountByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var count = _dataStore.UserInterests.Count(i => i.UserId == userId);
        return Task.FromResult(count);
    }
}
