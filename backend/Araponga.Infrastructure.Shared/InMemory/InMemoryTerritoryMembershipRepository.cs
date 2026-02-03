using Araponga.Application.Interfaces;
using Araponga.Domain.Membership;

namespace Araponga.Infrastructure.Shared.InMemory;

/// <summary>Implementação InMemory de ITerritoryMembershipRepository usando InMemorySharedStore.</summary>
public sealed class InMemoryTerritoryMembershipRepository : ITerritoryMembershipRepository
{
    private readonly InMemorySharedStore _store;

    public InMemoryTerritoryMembershipRepository(InMemorySharedStore store) => _store = store;

    public Task<TerritoryMembership?> GetByUserAndTerritoryAsync(Guid userId, Guid territoryId, CancellationToken cancellationToken)
        => Task.FromResult(_store.Memberships.FirstOrDefault(m => m.UserId == userId && m.TerritoryId == territoryId));

    public Task<TerritoryMembership?> GetByIdAsync(Guid membershipId, CancellationToken cancellationToken)
        => Task.FromResult(_store.Memberships.FirstOrDefault(m => m.Id == membershipId));

    public Task<bool> HasValidatedResidentAsync(Guid territoryId, CancellationToken cancellationToken)
        => Task.FromResult(_store.Memberships.Any(m => m.TerritoryId == territoryId && m.Role == MembershipRole.Resident && m.ResidencyVerification != ResidencyVerification.None));

    public Task<IReadOnlyList<Guid>> ListResidentUserIdsAsync(Guid territoryId, CancellationToken cancellationToken)
        => Task.FromResult<IReadOnlyList<Guid>>(_store.Memberships
            .Where(m => m.TerritoryId == territoryId && m.Role == MembershipRole.Resident && m.ResidencyVerification != ResidencyVerification.None)
            .Select(m => m.UserId).Distinct().ToList());

    public Task AddAsync(TerritoryMembership membership, CancellationToken cancellationToken)
    {
        _store.Memberships.Add(membership);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(TerritoryMembership membership, CancellationToken cancellationToken) => Task.CompletedTask;

    public Task UpdateRoleAsync(Guid membershipId, MembershipRole role, CancellationToken cancellationToken)
    {
        var m = _store.Memberships.FirstOrDefault(x => x.Id == membershipId);
        m?.UpdateRole(role);
        return Task.CompletedTask;
    }

    public Task<bool> HasResidentMembershipAsync(Guid userId, CancellationToken cancellationToken)
        => Task.FromResult(_store.Memberships.Any(m => m.UserId == userId && m.Role == MembershipRole.Resident));

    public Task<TerritoryMembership?> GetResidentMembershipAsync(Guid userId, CancellationToken cancellationToken)
        => Task.FromResult(_store.Memberships.FirstOrDefault(m => m.UserId == userId && m.Role == MembershipRole.Resident));

    public Task<IReadOnlyList<TerritoryMembership>> ListByUserAsync(Guid userId, CancellationToken cancellationToken)
        => Task.FromResult<IReadOnlyList<TerritoryMembership>>(_store.Memberships.Where(m => m.UserId == userId).ToList());

    public Task UpdateResidencyVerificationAsync(Guid membershipId, ResidencyVerification verification, CancellationToken cancellationToken)
    {
        var m = _store.Memberships.FirstOrDefault(x => x.Id == membershipId);
        m?.UpdateResidencyVerification(verification);
        return Task.CompletedTask;
    }

    public Task UpdateGeoVerificationAsync(Guid membershipId, DateTime verifiedAtUtc, CancellationToken cancellationToken)
    {
        var m = _store.Memberships.FirstOrDefault(x => x.Id == membershipId);
        m?.AddGeoVerification(verifiedAtUtc);
        return Task.CompletedTask;
    }

    public Task UpdateDocumentVerificationAsync(Guid membershipId, DateTime verifiedAtUtc, CancellationToken cancellationToken)
    {
        var m = _store.Memberships.FirstOrDefault(x => x.Id == membershipId);
        m?.AddDocumentVerification(verifiedAtUtc);
        return Task.CompletedTask;
    }
}
