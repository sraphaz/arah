using Araponga.Application.Interfaces;
using Araponga.Domain.Social;
using Araponga.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresTerritoryMembershipRepository : ITerritoryMembershipRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresTerritoryMembershipRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TerritoryMembership?> GetByUserAndTerritoryAsync(
        Guid userId,
        Guid territoryId,
        CancellationToken cancellationToken)
    {
        var record = await _dbContext.TerritoryMemberships
            .AsNoTracking()
            .FirstOrDefaultAsync(
                membership => membership.UserId == userId && membership.TerritoryId == territoryId,
                cancellationToken);
        return record?.ToDomain();
    }

    public async Task<TerritoryMembership?> GetByIdAsync(Guid membershipId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.TerritoryMemberships
            .AsNoTracking()
            .FirstOrDefaultAsync(membership => membership.Id == membershipId, cancellationToken);
        return record?.ToDomain();
    }

    public async Task AddAsync(TerritoryMembership membership, CancellationToken cancellationToken)
    {
        _dbContext.TerritoryMemberships.Add(membership.ToRecord());
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateStatusAsync(Guid membershipId, VerificationStatus status, CancellationToken cancellationToken)
    {
        var membership = await _dbContext.TerritoryMemberships
            .FirstOrDefaultAsync(m => m.Id == membershipId, cancellationToken);

        if (membership is null)
        {
            return;
        }

        membership.VerificationStatus = status;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateRoleAndStatusAsync(
        Guid membershipId,
        MembershipRole role,
        VerificationStatus status,
        CancellationToken cancellationToken)
    {
        var membership = await _dbContext.TerritoryMemberships
            .FirstOrDefaultAsync(m => m.Id == membershipId, cancellationToken);

        if (membership is null)
        {
            return;
        }

        membership.Role = role;
        membership.VerificationStatus = status;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
