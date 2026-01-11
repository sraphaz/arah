using Araponga.Application.Interfaces;
using Araponga.Domain.Moderation;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresSanctionRepository : ISanctionRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresSanctionRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Sanction sanction, CancellationToken cancellationToken)
    {
        _dbContext.Sanctions.Add(new Entities.SanctionRecord
        {
            Id = sanction.Id,
            TerritoryId = sanction.TerritoryId,
            Scope = sanction.Scope,
            TargetType = sanction.TargetType,
            TargetId = sanction.TargetId,
            Type = sanction.Type,
            Reason = sanction.Reason,
            Status = sanction.Status,
            StartAtUtc = sanction.StartAtUtc,
            EndAtUtc = sanction.EndAtUtc,
            CreatedAtUtc = sanction.CreatedAtUtc
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Sanction>> ListActiveForTargetAsync(
        Guid targetId,
        DateTime referenceUtc,
        CancellationToken cancellationToken)
    {
        var records = await _dbContext.Sanctions
            .AsNoTracking()
            .Where(sanction =>
                sanction.TargetId == targetId &&
                sanction.Status == SanctionStatus.Active &&
                sanction.StartAtUtc <= referenceUtc &&
                (sanction.EndAtUtc == null || sanction.EndAtUtc >= referenceUtc))
            .ToListAsync(cancellationToken);

        return records.Select(record => new Sanction(
            record.Id,
            record.TerritoryId,
            record.Scope,
            record.TargetType,
            record.TargetId,
            record.Type,
            record.Reason,
            record.Status,
            record.StartAtUtc,
            record.EndAtUtc,
            record.CreatedAtUtc)).ToList();
    }

    public async Task<bool> HasActiveSanctionAsync(
        Guid targetId,
        Guid territoryId,
        SanctionType type,
        DateTime referenceUtc,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Sanctions
            .AsNoTracking()
            .AnyAsync(sanction =>
                sanction.TargetId == targetId &&
                sanction.Type == type &&
                sanction.Status == SanctionStatus.Active &&
                sanction.StartAtUtc <= referenceUtc &&
                (sanction.EndAtUtc == null || sanction.EndAtUtc >= referenceUtc) &&
                (sanction.Scope == SanctionScope.Global ||
                 (sanction.Scope == SanctionScope.Territory && sanction.TerritoryId == territoryId)),
                cancellationToken);
    }
}
