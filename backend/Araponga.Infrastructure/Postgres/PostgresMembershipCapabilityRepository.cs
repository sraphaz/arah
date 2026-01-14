using Araponga.Application.Interfaces;
using Araponga.Domain.Membership;
using Araponga.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresMembershipCapabilityRepository : IMembershipCapabilityRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresMembershipCapabilityRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<MembershipCapability?> GetByIdAsync(Guid capabilityId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.MembershipCapabilities
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == capabilityId, cancellationToken);
        return record?.ToDomain();
    }

    public async Task<IReadOnlyList<MembershipCapability>> GetByMembershipIdAsync(Guid membershipId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.MembershipCapabilities
            .AsNoTracking()
            .Where(c => c.MembershipId == membershipId)
            .ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<MembershipCapability>> GetActiveByMembershipIdAsync(Guid membershipId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.MembershipCapabilities
            .AsNoTracking()
            .Where(c => c.MembershipId == membershipId && c.RevokedAtUtc == null)
            .ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public async Task<bool> HasCapabilityAsync(Guid membershipId, MembershipCapabilityType capabilityType, CancellationToken cancellationToken)
    {
        return await _dbContext.MembershipCapabilities
            .AsNoTracking()
            .AnyAsync(
                c => c.MembershipId == membershipId &&
                     c.CapabilityType == capabilityType &&
                     c.RevokedAtUtc == null,
                cancellationToken);
    }

    public Task AddAsync(MembershipCapability capability, CancellationToken cancellationToken)
    {
        _dbContext.MembershipCapabilities.Add(capability.ToRecord());
        return Task.CompletedTask;
    }

    public async Task UpdateAsync(MembershipCapability capability, CancellationToken cancellationToken)
    {
        var record = await _dbContext.MembershipCapabilities
            .FirstOrDefaultAsync(c => c.Id == capability.Id, cancellationToken);

        if (record is null)
        {
            _dbContext.MembershipCapabilities.Add(capability.ToRecord());
        }
        else
        {
            var updatedRecord = capability.ToRecord();
            _dbContext.Entry(record).CurrentValues.SetValues(updatedRecord);
        }
    }

    public async Task<IReadOnlyList<Guid>> ListMembershipIdsWithCapabilityAsync(
        MembershipCapabilityType capabilityType,
        Guid territoryId,
        CancellationToken cancellationToken)
    {
        var membershipIds = await _dbContext.MembershipCapabilities
            .AsNoTracking()
            .Where(c => c.CapabilityType == capabilityType && c.RevokedAtUtc == null)
            .Join(
                _dbContext.TerritoryMemberships,
                capability => capability.MembershipId,
                membership => membership.Id,
                (capability, membership) => new { membership.TerritoryId, membership.Id })
            .Where(x => x.TerritoryId == territoryId)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        return membershipIds;
    }
}
