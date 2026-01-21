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
    
    /// <summary>
    /// Lists events by territory with pagination.
    /// </summary>
    Task<IReadOnlyList<TerritoryEvent>> ListByTerritoryPagedAsync(
        Guid territoryId,
        DateTime? fromUtc,
        DateTime? toUtc,
        EventStatus? status,
        int skip,
        int take,
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Lists events by bounding box with pagination.
    /// </summary>
    Task<IReadOnlyList<TerritoryEvent>> ListByBoundingBoxPagedAsync(
        double minLatitude,
        double maxLatitude,
        double minLongitude,
        double maxLongitude,
        DateTime? fromUtc,
        DateTime? toUtc,
        Guid? territoryId,
        int skip,
        int take,
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Counts events by territory.
    /// </summary>
    Task<int> CountByTerritoryAsync(
        Guid territoryId,
        DateTime? fromUtc,
        DateTime? toUtc,
        EventStatus? status,
        CancellationToken cancellationToken);
    
    Task AddAsync(TerritoryEvent territoryEvent, CancellationToken cancellationToken);
    Task UpdateAsync(TerritoryEvent territoryEvent, CancellationToken cancellationToken);
    
    /// <summary>
    /// Lists events created by a user with pagination.
    /// </summary>
    Task<IReadOnlyList<TerritoryEvent>> ListByAuthorPagedAsync(
        Guid userId,
        int skip,
        int take,
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Counts events created by a user.
    /// </summary>
    Task<int> CountByAuthorAsync(Guid userId, CancellationToken cancellationToken);
}
