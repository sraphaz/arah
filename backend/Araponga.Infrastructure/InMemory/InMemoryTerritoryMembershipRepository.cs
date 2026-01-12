using Araponga.Application.Interfaces;
using Araponga.Domain.Social;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryTerritoryMembershipRepository : ITerritoryMembershipRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryTerritoryMembershipRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<TerritoryMembership?> GetByUserAndTerritoryAsync(
        Guid userId,
        Guid territoryId,
        CancellationToken cancellationToken)
    {
        var membership = _dataStore.Memberships.FirstOrDefault(m =>
            m.UserId == userId && m.TerritoryId == territoryId);

        return Task.FromResult(membership);
    }

    public Task<TerritoryMembership?> GetByIdAsync(Guid membershipId, CancellationToken cancellationToken)
    {
        var membership = _dataStore.Memberships.FirstOrDefault(m => m.Id == membershipId);
        return Task.FromResult(membership);
    }

    public Task<bool> HasValidatedResidentAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        var hasResident = _dataStore.Memberships.Any(m =>
            m.TerritoryId == territoryId &&
            m.Role == MembershipRole.Resident &&
            m.VerificationStatus == VerificationStatus.Validated);

        return Task.FromResult(hasResident);
    }

    public Task<IReadOnlyList<Guid>> ListResidentUserIdsAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        var residents = _dataStore.Memberships
            .Where(m => m.TerritoryId == territoryId &&
                        m.Role == MembershipRole.Resident &&
                        m.VerificationStatus == VerificationStatus.Validated)
            .Select(m => m.UserId)
            .Distinct()
            .ToList();

        return Task.FromResult<IReadOnlyList<Guid>>(residents);
    }

    public Task AddAsync(TerritoryMembership membership, CancellationToken cancellationToken)
    {
        _dataStore.Memberships.Add(membership);
        return Task.CompletedTask;
    }

    public Task UpdateStatusAsync(Guid membershipId, VerificationStatus status, CancellationToken cancellationToken)
    {
        var membership = _dataStore.Memberships.FirstOrDefault(m => m.Id == membershipId);
        if (membership is null)
        {
            return Task.CompletedTask;
        }

        membership.UpdateVerificationStatus(status);
        return Task.CompletedTask;
    }

    public Task UpdateRoleAndStatusAsync(
        Guid membershipId,
        MembershipRole role,
        VerificationStatus status,
        CancellationToken cancellationToken)
    {
        var membership = _dataStore.Memberships.FirstOrDefault(m => m.Id == membershipId);
        if (membership is null)
        {
            return Task.CompletedTask;
        }

        membership.UpdateRole(role);
        membership.UpdateVerificationStatus(status);
        return Task.CompletedTask;
    }
}
