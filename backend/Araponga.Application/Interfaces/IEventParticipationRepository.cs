using Araponga.Application.Models;
using Araponga.Domain.Events;

namespace Araponga.Application.Interfaces;

public interface IEventParticipationRepository
{
    Task<EventParticipation?> GetAsync(Guid eventId, Guid userId, CancellationToken cancellationToken);
    Task UpsertAsync(EventParticipation participation, CancellationToken cancellationToken);
    Task<IReadOnlyList<EventParticipationCounts>> GetCountsAsync(
        IReadOnlyCollection<Guid> eventIds,
        CancellationToken cancellationToken);
}
