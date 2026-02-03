using Araponga.Domain.Map;

namespace Araponga.Application.Interfaces;

public interface IMapRepository
{
    Task<IReadOnlyList<MapEntity>> ListByTerritoryAsync(Guid territoryId, CancellationToken cancellationToken);
    Task<MapEntity?> GetByIdAsync(Guid entityId, CancellationToken cancellationToken);
    Task AddAsync(MapEntity entity, CancellationToken cancellationToken);
    Task UpdateStatusAsync(Guid entityId, MapEntityStatus status, CancellationToken cancellationToken);
    Task IncrementConfirmationAsync(Guid entityId, CancellationToken cancellationToken);
    
    /// <summary>
    /// Lists map entities by territory with pagination.
    /// </summary>
    Task<IReadOnlyList<MapEntity>> ListByTerritoryPagedAsync(
        Guid territoryId,
        int skip,
        int take,
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Counts map entities by territory.
    /// </summary>
    Task<int> CountByTerritoryAsync(Guid territoryId, CancellationToken cancellationToken);
}
