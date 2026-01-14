using Araponga.Domain.Membership;

namespace Araponga.Application.Interfaces;

public interface IMembershipSettingsRepository
{
    Task<MembershipSettings?> GetByMembershipIdAsync(Guid membershipId, CancellationToken cancellationToken);
    Task AddAsync(MembershipSettings settings, CancellationToken cancellationToken);
    Task UpdateAsync(MembershipSettings settings, CancellationToken cancellationToken);
}
