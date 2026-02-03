using Araponga.Application.Interfaces;
using Araponga.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Shared.Postgres;

/// <summary>Implementação Postgres de IUserInterestRepository usando SharedDbContext.</summary>
public sealed class PostgresUserInterestRepository : IUserInterestRepository
{
    private readonly SharedDbContext _dbContext;

    public PostgresUserInterestRepository(SharedDbContext dbContext) => _dbContext = dbContext;

    public Task AddAsync(UserInterest interest, CancellationToken cancellationToken)
    {
        _dbContext.UserInterests.Add(interest.ToRecord());
        return Task.CompletedTask;
    }

    public async Task RemoveAsync(Guid userId, string interestTag, CancellationToken cancellationToken)
    {
        var normalizedTag = interestTag.Trim().ToLowerInvariant();
        var record = await _dbContext.UserInterests
            .FirstOrDefaultAsync(i => i.UserId == userId && i.InterestTag == normalizedTag, cancellationToken);
        if (record is not null)
            _dbContext.UserInterests.Remove(record);
    }

    public async Task<IReadOnlyList<UserInterest>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.UserInterests
            .AsNoTracking()
            .Where(i => i.UserId == userId)
            .OrderBy(i => i.InterestTag)
            .ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<Guid>> ListUserIdsByInterestAsync(string interestTag, Guid territoryId, CancellationToken cancellationToken)
    {
        var normalizedTag = interestTag.Trim().ToLowerInvariant();
        var userIds = await _dbContext.UserInterests
            .AsNoTracking()
            .Where(i => i.InterestTag == normalizedTag)
            .Join(
                _dbContext.TerritoryMemberships.Where(m => m.TerritoryId == territoryId),
                interest => interest.UserId,
                membership => membership.UserId,
                (interest, _) => interest.UserId)
            .Distinct()
            .ToListAsync(cancellationToken);
        return userIds;
    }

    public async Task<bool> ExistsAsync(Guid userId, string interestTag, CancellationToken cancellationToken)
    {
        var normalizedTag = interestTag.Trim().ToLowerInvariant();
        return await _dbContext.UserInterests
            .AsNoTracking()
            .AnyAsync(i => i.UserId == userId && i.InterestTag == normalizedTag, cancellationToken);
    }

    public async Task<int> CountByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _dbContext.UserInterests
            .AsNoTracking()
            .CountAsync(i => i.UserId == userId, cancellationToken);
    }
}
