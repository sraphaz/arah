using Araponga.Domain.Events;

namespace Araponga.Application.Interfaces;

public interface ITerritoryEventRepository
{
    Task<TerritoryEvent?> GetByIdAsync(Guid eventId, CancellationToken cancellationToken);
    Task<IReadOnlyList<TerritoryEvent>> ListByIdsAsync(IReadOnlyCollection<Guid> eventIds, CancellationToken cancellationToken);
    Task<IReadOnlyList<TerritoryEvent>> ListByTerritoryAsync(
        Guid territoryId,
        DateTime? fromUtc,
        DateTime? toUtc,
        EventStatus? status,
        CancellationToken cancellationToken);
    Task<IReadOnlyList<TerritoryEvent>> ListByBoundingBoxAsync(
        double minLatitude,
        double maxLatitude,
        double minLongitude,
        double maxLongitude,
        DateTime? fromUtc,
        DateTime? toUtc,
        Guid? territoryId,
        CancellationToken cancellationToken);
    Task AddAsync(TerritoryEvent territoryEvent, CancellationToken cancellationToken);
    Task UpdateAsync(TerritoryEvent territoryEvent, CancellationToken cancellationToken);
}
