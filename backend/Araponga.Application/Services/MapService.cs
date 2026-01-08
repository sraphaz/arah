using Araponga.Application.Interfaces;
using Araponga.Domain.Map;

namespace Araponga.Application.Services;

public sealed class MapService
{
    private readonly IMapRepository _mapRepository;
    private readonly AccessEvaluator _accessEvaluator;

    public MapService(IMapRepository mapRepository, AccessEvaluator accessEvaluator)
    {
        _mapRepository = mapRepository;
        _accessEvaluator = accessEvaluator;
    }

    public async Task<IReadOnlyList<MapEntity>> ListEntitiesAsync(
        Guid territoryId,
        Guid? userId,
        CancellationToken cancellationToken)
    {
        var entities = await _mapRepository.ListByTerritoryAsync(territoryId, cancellationToken);

        if (userId is null)
        {
            return entities
                .Where(entity => entity.Visibility == MapEntityVisibility.Public)
                .ToList();
        }

        var isResident = await _accessEvaluator.IsResidentAsync(userId.Value, territoryId, cancellationToken);

        return isResident
            ? entities.ToList()
            : entities.Where(entity => entity.Visibility == MapEntityVisibility.Public).ToList();
    }
}
