using Araponga.Domain.Membership;

namespace Araponga.Application.Interfaces;

public interface IMembershipCapabilityRepository
{
    Task<MembershipCapability?> GetByIdAsync(Guid capabilityId, CancellationToken cancellationToken);
    Task<IReadOnlyList<MembershipCapability>> GetByMembershipIdAsync(Guid membershipId, CancellationToken cancellationToken);
    Task<IReadOnlyList<MembershipCapability>> GetActiveByMembershipIdAsync(Guid membershipId, CancellationToken cancellationToken);
    Task<bool> HasCapabilityAsync(Guid membershipId, MembershipCapabilityType capabilityType, CancellationToken cancellationToken);
    Task AddAsync(MembershipCapability capability, CancellationToken cancellationToken);
    Task UpdateAsync(MembershipCapability capability, CancellationToken cancellationToken);
    Task<IReadOnlyList<Guid>> ListMembershipIdsWithCapabilityAsync(MembershipCapabilityType capabilityType, Guid territoryId, CancellationToken cancellationToken);
}
