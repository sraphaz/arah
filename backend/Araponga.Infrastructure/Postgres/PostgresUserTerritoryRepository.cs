using Araponga.Application.Interfaces;
using Araponga.Domain.Users;
using Araponga.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresUserTerritoryRepository : IUserTerritoryRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresUserTerritoryRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserTerritory?> GetByUserAndTerritoryAsync(
        Guid userId,
        Guid territoryId,
        CancellationToken cancellationToken)
    {
        var record = await _dbContext.UserTerritories
            .AsNoTracking()
            .FirstOrDefaultAsync(
                membership => membership.UserId == userId && membership.TerritoryId == territoryId,
                cancellationToken);
        return record?.ToDomain();
    }

    public Task AddAsync(UserTerritory membership, CancellationToken cancellationToken)
    {
        _dbContext.UserTerritories.Add(membership.ToRecord());
        return Task.CompletedTask;
    }

    public async Task<bool> IsValidatedAsync(Guid userId, Guid territoryId, CancellationToken cancellationToken)
    {
        return await _dbContext.UserTerritories
            .AsNoTracking()
            .AnyAsync(
                membership => membership.UserId == userId &&
                              membership.TerritoryId == territoryId &&
                              membership.Status == MembershipStatus.Validated,
                cancellationToken);
    }
}
