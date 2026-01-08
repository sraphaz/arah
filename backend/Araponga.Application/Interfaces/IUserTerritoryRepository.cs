using Araponga.Domain.Users;

namespace Araponga.Application.Interfaces;

public interface IUserTerritoryRepository
{
    Task<UserTerritory?> GetByUserAndTerritoryAsync(Guid userId, Guid territoryId, CancellationToken cancellationToken);
    Task AddAsync(UserTerritory membership, CancellationToken cancellationToken);
    Task<bool> IsValidatedAsync(Guid userId, Guid territoryId, CancellationToken cancellationToken);
}
