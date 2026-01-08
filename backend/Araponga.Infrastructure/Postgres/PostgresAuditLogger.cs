using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Infrastructure.Postgres.Entities;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresAuditLogger : IAuditLogger
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresAuditLogger(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task LogAsync(AuditEntry entry, CancellationToken cancellationToken)
    {
        _dbContext.AuditEntries.Add(new AuditEntryRecord
        {
            Id = Guid.NewGuid(),
            Action = entry.Action,
            ActorUserId = entry.ActorUserId,
            TerritoryId = entry.TerritoryId,
            TargetId = entry.TargetId,
            TimestampUtc = entry.TimestampUtc
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
