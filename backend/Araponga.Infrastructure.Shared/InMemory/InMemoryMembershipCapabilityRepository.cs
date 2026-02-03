using Araponga.Application.Interfaces;
using Araponga.Domain.Membership;

namespace Araponga.Infrastructure.Shared.InMemory;

/// <summary>Implementação InMemory de IMembershipCapabilityRepository usando InMemorySharedStore.</summary>
public sealed class InMemoryMembershipCapabilityRepository : IMembershipCapabilityRepository
{
    private readonly InMemorySharedStore _store;

    public InMemoryMembershipCapabilityRepository(InMemorySharedStore store) => _store = store;

    public Task<MembershipCapability?> GetByIdAsync(Guid capabilityId, CancellationToken cancellationToken)
        => Task.FromResult(_store.MembershipCapabilities.FirstOrDefault(c => c.Id == capabilityId));

    public Task<IReadOnlyList<MembershipCapability>> GetByMembershipIdAsync(Guid membershipId, CancellationToken cancellationToken)
        => Task.FromResult<IReadOnlyList<MembershipCapability>>(_store.MembershipCapabilities.Where(c => c.MembershipId == membershipId).ToList());

    public Task<IReadOnlyList<MembershipCapability>> GetActiveByMembershipIdAsync(Guid membershipId, CancellationToken cancellationToken)
        => Task.FromResult<IReadOnlyList<MembershipCapability>>(_store.MembershipCapabilities.Where(c => c.MembershipId == membershipId && c.IsActive()).ToList());

    public Task<bool> HasCapabilityAsync(Guid membershipId, MembershipCapabilityType capabilityType, CancellationToken cancellationToken)
        => Task.FromResult(_store.MembershipCapabilities.Any(c => c.MembershipId == membershipId && c.CapabilityType == capabilityType && c.IsActive()));

    public Task AddAsync(MembershipCapability capability, CancellationToken cancellationToken)
    {
        _store.MembershipCapabilities.Add(capability);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(MembershipCapability capability, CancellationToken cancellationToken)
    {
        var existing = _store.MembershipCapabilities.FirstOrDefault(c => c.Id == capability.Id);
        if (existing is null) _store.MembershipCapabilities.Add(capability);
        else { var i = _store.MembershipCapabilities.IndexOf(existing); if (i >= 0) _store.MembershipCapabilities[i] = capability; }
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Guid>> ListMembershipIdsWithCapabilityAsync(MembershipCapabilityType capabilityType, Guid territoryId, CancellationToken cancellationToken)
    {
        var ids = _store.MembershipCapabilities
            .Where(c => c.CapabilityType == capabilityType && c.IsActive())
            .Select(c => c.MembershipId)
            .Distinct()
            .ToList();
        var filtered = _store.Memberships.Where(m => m.TerritoryId == territoryId && ids.Contains(m.Id)).Select(m => m.Id).ToList();
        return Task.FromResult<IReadOnlyList<Guid>>(filtered);
    }
}
