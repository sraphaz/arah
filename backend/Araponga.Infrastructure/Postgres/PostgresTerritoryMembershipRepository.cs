using Araponga.Application.Interfaces;
using Araponga.Domain.Membership;
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

    public Task<bool> HasValidatedResidentAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        return _dbContext.TerritoryMemberships
            .AsNoTracking()
            .AnyAsync(
                membership => membership.TerritoryId == territoryId &&
                              membership.Role == MembershipRole.Resident &&
                              membership.ResidencyVerification != ResidencyVerification.None,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Guid>> ListResidentUserIdsAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        var userIds = await _dbContext.TerritoryMemberships
            .AsNoTracking()
            .Where(membership => membership.TerritoryId == territoryId &&
                                 membership.Role == MembershipRole.Resident &&
                                 membership.ResidencyVerification != ResidencyVerification.None)
            .Select(membership => membership.UserId)
            .Distinct()
            .ToListAsync(cancellationToken);

        return userIds;
    }

    public Task AddAsync(TerritoryMembership membership, CancellationToken cancellationToken)
    {
        _dbContext.TerritoryMemberships.Add(membership.ToRecord());
        return Task.CompletedTask;
    }

    public async Task UpdateAsync(TerritoryMembership membership, CancellationToken cancellationToken)
    {
        var record = await _dbContext.TerritoryMemberships
            .FirstOrDefaultAsync(m => m.Id == membership.Id, cancellationToken);

        if (record is null)
        {
            return;
        }

        // Atualizar todas as propriedades da entidade de domínio
        record.Role = membership.Role;
        record.ResidencyVerification = membership.ResidencyVerification;
        record.LastGeoVerifiedAtUtc = membership.LastGeoVerifiedAtUtc;
        record.LastDocumentVerifiedAtUtc = membership.LastDocumentVerifiedAtUtc;
    }

    public async Task UpdateRoleAsync(Guid membershipId, MembershipRole role, CancellationToken cancellationToken)
    {
        var membership = await _dbContext.TerritoryMemberships
            .FirstOrDefaultAsync(m => m.Id == membershipId, cancellationToken);

        if (membership is null)
        {
            return;
        }

        membership.Role = role;
    }

    public Task<bool> HasResidentMembershipAsync(Guid userId, CancellationToken cancellationToken)
    {
        return _dbContext.TerritoryMemberships
            .AsNoTracking()
            .AnyAsync(
                membership => membership.UserId == userId &&
                              membership.Role == MembershipRole.Resident,
                cancellationToken);
    }

    public async Task<TerritoryMembership?> GetResidentMembershipAsync(Guid userId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.TerritoryMemberships
            .AsNoTracking()
            .FirstOrDefaultAsync(
                membership => membership.UserId == userId &&
                              membership.Role == MembershipRole.Resident,
                cancellationToken);
        return record?.ToDomain();
    }

    public async Task<IReadOnlyList<TerritoryMembership>> ListByUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.TerritoryMemberships
            .AsNoTracking()
            .Where(membership => membership.UserId == userId)
            .ToListAsync(cancellationToken);

        return records.Select(record => record.ToDomain()).ToList();
    }

    public async Task UpdateResidencyVerificationAsync(Guid membershipId, ResidencyVerification verification, CancellationToken cancellationToken)
    {
        var membership = await _dbContext.TerritoryMemberships
            .FirstOrDefaultAsync(m => m.Id == membershipId, cancellationToken);

        if (membership is null)
        {
            return;
        }

        membership.ResidencyVerification = verification;
    }

    public async Task UpdateGeoVerificationAsync(Guid membershipId, DateTime verifiedAtUtc, CancellationToken cancellationToken)
    {
        var record = await _dbContext.TerritoryMemberships
            .FirstOrDefaultAsync(m => m.Id == membershipId, cancellationToken);

        if (record is null)
        {
            return;
        }

        var membership = record.ToDomain();
        membership.AddGeoVerification(verifiedAtUtc);
        
        var updatedRecord = membership.ToRecord();
        _dbContext.Entry(record).CurrentValues.SetValues(updatedRecord);
    }

    public async Task UpdateDocumentVerificationAsync(Guid membershipId, DateTime verifiedAtUtc, CancellationToken cancellationToken)
    {
        var record = await _dbContext.TerritoryMemberships
            .FirstOrDefaultAsync(m => m.Id == membershipId, cancellationToken);

        if (record is null)
        {
            return;
        }

        var membership = record.ToDomain();
        membership.AddDocumentVerification(verifiedAtUtc);
        
        var updatedRecord = membership.ToRecord();
        _dbContext.Entry(record).CurrentValues.SetValues(updatedRecord);
    }
}
