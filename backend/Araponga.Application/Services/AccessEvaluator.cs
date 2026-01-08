using Araponga.Application.Interfaces;

namespace Araponga.Application.Services;

public sealed class AccessEvaluator
{
    private readonly IUserTerritoryRepository _userTerritoryRepository;

    public AccessEvaluator(IUserTerritoryRepository userTerritoryRepository)
    {
        _userTerritoryRepository = userTerritoryRepository;
    }

    public Task<bool> IsResidentAsync(Guid userId, Guid territoryId, CancellationToken cancellationToken)
    {
        return _userTerritoryRepository.IsValidatedAsync(userId, territoryId, cancellationToken);
    }
}
