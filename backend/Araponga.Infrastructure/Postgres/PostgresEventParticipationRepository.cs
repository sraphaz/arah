using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresEventParticipationRepository : IEventParticipationRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresEventParticipationRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<EventParticipation?> GetAsync(Guid eventId, Guid userId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.EventParticipations
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.EventId == eventId && p.UserId == userId, cancellationToken);

        if (record is null)
        {
            return null;
        }

        return new EventParticipation(
            record.EventId,
            record.UserId,
            record.Status,
            record.CreatedAtUtc,
            record.UpdatedAtUtc);
    }

    public async Task UpsertAsync(EventParticipation participation, CancellationToken cancellationToken)
    {
        var record = await _dbContext.EventParticipations
            .FirstOrDefaultAsync(p => p.EventId == participation.EventId && p.UserId == participation.UserId, cancellationToken);

        if (record is null)
        {
            _dbContext.EventParticipations.Add(participation.ToRecord());
            return;
        }

        record.Status = participation.Status;
        record.UpdatedAtUtc = participation.UpdatedAtUtc;
        _dbContext.EventParticipations.Update(record);
    }

    public async Task<IReadOnlyList<EventParticipationCounts>> GetCountsAsync(
        IReadOnlyCollection<Guid> eventIds,
        CancellationToken cancellationToken)
    {
        if (eventIds.Count == 0)
        {
            return Array.Empty<EventParticipationCounts>();
        }

        var counts = await _dbContext.EventParticipations
            .AsNoTracking()
            .Where(p => eventIds.Contains(p.EventId))
            .GroupBy(p => p.EventId)
            .Select(group => new
            {
                EventId = group.Key,
                Interested = group.Count(p => p.Status == EventParticipationStatus.Interested),
                Confirmed = group.Count(p => p.Status == EventParticipationStatus.Confirmed)
            })
            .ToListAsync(cancellationToken);

        return counts
            .Select(item => new EventParticipationCounts(item.EventId, item.Interested, item.Confirmed))
            .ToList();
    }
}
