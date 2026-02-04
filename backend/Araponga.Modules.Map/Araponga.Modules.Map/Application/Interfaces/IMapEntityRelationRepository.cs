using Araponga.Modules.Map.Domain;

namespace Araponga.Modules.Map.Application.Interfaces;

public interface IMapEntityRelationRepository
{
    Task<bool> ExistsAsync(Guid userId, Guid entityId, CancellationToken cancellationToken);
    Task AddAsync(MapEntityRelation relation, CancellationToken cancellationToken);
}
