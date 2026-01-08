using Araponga.Domain.Map;

namespace Araponga.Application.Interfaces;

public interface IMapRepository
{
    Task<IReadOnlyList<MapEntity>> ListByTerritoryAsync(Guid territoryId, CancellationToken cancellationToken);
}
