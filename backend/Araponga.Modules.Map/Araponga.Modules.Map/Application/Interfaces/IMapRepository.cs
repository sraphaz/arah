using Araponga.Modules.Map.Domain;

namespace Araponga.Modules.Map.Application.Interfaces;

public interface IMapRepository
{
    Task<IReadOnlyList<MapEntity>> ListByTerritoryAsync(Guid territoryId, CancellationToken cancellationToken);
    Task<MapEntity?> GetByIdAsync(Guid entityId, CancellationToken cancellationToken);
    Task AddAsync(MapEntity entity, CancellationToken cancellationToken);
    Task UpdateStatusAsync(Guid entityId, MapEntityStatus status, CancellationToken cancellationToken);
    Task IncrementConfirmationAsync(Guid entityId, CancellationToken cancellationToken);
    
    Task<IReadOnlyList<MapEntity>> ListByTerritoryPagedAsync(
        Guid territoryId,
        int skip,
        int take,
        CancellationToken cancellationToken);
    
    Task<int> CountByTerritoryAsync(Guid territoryId, CancellationToken cancellationToken);
}
