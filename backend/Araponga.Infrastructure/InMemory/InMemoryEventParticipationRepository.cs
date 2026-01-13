using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Domain.Events;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryEventParticipationRepository : IEventParticipationRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryEventParticipationRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<EventParticipation?> GetAsync(Guid eventId, Guid userId, CancellationToken cancellationToken)
    {
        var participation = _dataStore.EventParticipations
            .FirstOrDefault(p => p.EventId == eventId && p.UserId == userId);
        return Task.FromResult(participation);
    }

    public Task UpsertAsync(EventParticipation participation, CancellationToken cancellationToken)
    {
        var existing = _dataStore.EventParticipations
            .FirstOrDefault(p => p.EventId == participation.EventId && p.UserId == participation.UserId);

        if (existing is not null)
        {
            _dataStore.EventParticipations.Remove(existing);
        }

        _dataStore.EventParticipations.Add(participation);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<EventParticipationCounts>> GetCountsAsync(
        IReadOnlyCollection<Guid> eventIds,
        CancellationToken cancellationToken)
    {
        if (eventIds.Count == 0)
        {
            return Task.FromResult<IReadOnlyList<EventParticipationCounts>>(Array.Empty<EventParticipationCounts>());
        }

        var counts = _dataStore.EventParticipations
            .Where(p => eventIds.Contains(p.EventId))
            .GroupBy(p => p.EventId)
            .Select(group => new EventParticipationCounts(
                group.Key,
                group.Count(p => p.Status == EventParticipationStatus.Interested),
                group.Count(p => p.Status == EventParticipationStatus.Confirmed)))
            .ToList();

        return Task.FromResult<IReadOnlyList<EventParticipationCounts>>(counts);
    }
}
