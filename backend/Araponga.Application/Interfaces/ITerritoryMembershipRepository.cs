using Araponga.Domain.Social;

namespace Araponga.Application.Interfaces;

public interface ITerritoryMembershipRepository
{
    Task<TerritoryMembership?> GetByUserAndTerritoryAsync(Guid userId, Guid territoryId, CancellationToken cancellationToken);
    Task<TerritoryMembership?> GetByIdAsync(Guid membershipId, CancellationToken cancellationToken);
    Task<bool> HasValidatedResidentAsync(Guid territoryId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Guid>> ListResidentUserIdsAsync(Guid territoryId, CancellationToken cancellationToken);
    Task AddAsync(TerritoryMembership membership, CancellationToken cancellationToken);
    Task UpdateStatusAsync(Guid membershipId, VerificationStatus status, CancellationToken cancellationToken);
    Task UpdateRoleAndStatusAsync(Guid membershipId, MembershipRole role, VerificationStatus status, CancellationToken cancellationToken);
}
