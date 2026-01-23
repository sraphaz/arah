using Araponga.Application.Interfaces;
using Araponga.Domain.Users;
using Araponga.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresUserInterestRepository : IUserInterestRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresUserInterestRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(UserInterest interest, CancellationToken cancellationToken)
    {
        var record = new UserInterestRecord
        {
            Id = interest.Id,
            UserId = interest.UserId,
            InterestTag = interest.InterestTag,
            CreatedAtUtc = interest.CreatedAtUtc
        };
        _dbContext.UserInterests.Add(record);
        return Task.CompletedTask;
    }

    public async Task RemoveAsync(Guid userId, string interestTag, CancellationToken cancellationToken)
    {
        var normalizedTag = interestTag.Trim().ToLowerInvariant();
        var record = await _dbContext.UserInterests
            .FirstOrDefaultAsync(
                i => i.UserId == userId && i.InterestTag == normalizedTag,
                cancellationToken);
        
        if (record is not null)
        {
            _dbContext.UserInterests.Remove(record);
        }
    }

    public async Task<IReadOnlyList<UserInterest>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.UserInterests
            .AsNoTracking()
            .Where(i => i.UserId == userId)
            .OrderBy(i => i.InterestTag)
            .ToListAsync(cancellationToken);

        return records.Select(r => new UserInterest(r.Id, r.UserId, r.InterestTag, r.CreatedAtUtc)).ToList();
    }

    public async Task<IReadOnlyList<Guid>> ListUserIdsByInterestAsync(string interestTag, Guid territoryId, CancellationToken cancellationToken)
    {
        var normalizedTag = interestTag.Trim().ToLowerInvariant();
        
        // Buscar usuários que têm o interesse E são membros do território
        var userIds = await _dbContext.UserInterests
            .AsNoTracking()
            .Where(i => i.InterestTag == normalizedTag)
            .Join(
                _dbContext.TerritoryMemberships.Where(m => m.TerritoryId == territoryId),
                interest => interest.UserId,
                membership => membership.UserId,
                (interest, membership) => interest.UserId)
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
