using Araponga.Application.Interfaces;
using Araponga.Domain.Users;

namespace Araponga.Application.Services;

public sealed class MembershipService
{
    private readonly IUserTerritoryRepository _userTerritoryRepository;

    public MembershipService(IUserTerritoryRepository userTerritoryRepository)
    {
        _userTerritoryRepository = userTerritoryRepository;
    }

    public async Task<UserTerritory> DeclareResidentAsync(
        Guid userId,
        Guid territoryId,
        CancellationToken cancellationToken)
    {
        var existing = await _userTerritoryRepository.GetByUserAndTerritoryAsync(
            userId,
            territoryId,
            cancellationToken);

        if (existing is not null)
        {
            return existing;
        }

        var membership = new UserTerritory(
            Guid.NewGuid(),
            userId,
            territoryId,
            MembershipStatus.Pending,
            DateTime.UtcNow);

        await _userTerritoryRepository.AddAsync(membership, cancellationToken);

        return membership;
    }
}
