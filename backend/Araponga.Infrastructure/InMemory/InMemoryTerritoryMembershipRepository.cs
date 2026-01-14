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
            m.ResidencyVerification != ResidencyVerification.Unverified);

        return Task.FromResult(hasResident);
    }

    public Task<IReadOnlyList<Guid>> ListResidentUserIdsAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        var residents = _dataStore.Memberships
            .Where(m => m.TerritoryId == territoryId &&
                        m.Role == MembershipRole.Resident &&
                        m.ResidencyVerification != ResidencyVerification.Unverified)
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

    public Task UpdateAsync(TerritoryMembership membership, CancellationToken cancellationToken)
    {
        // No in-memory, as entidades são compartilhadas por referência.
        // Quando modificamos a entidade usando métodos Update* do domínio,
        // a referência no _dataStore já reflete as mudanças automaticamente.
        // Este método existe apenas para manter a interface consistente
        // com a implementação Postgres e para garantir que a entidade existe.
        var existing = _dataStore.Memberships.FirstOrDefault(m => m.Id == membership.Id);
        if (existing is null)
        {
            return Task.CompletedTask;
        }

        // As modificações já foram aplicadas na entidade de domínio via métodos Update*.
        // No in-memory, não precisamos fazer nada adicional pois a referência é compartilhada.
        return Task.CompletedTask;
    }

    public Task UpdateRoleAsync(Guid membershipId, MembershipRole role, CancellationToken cancellationToken)
    {
        var membership = _dataStore.Memberships.FirstOrDefault(m => m.Id == membershipId);
        if (membership is null)
        {
            return Task.CompletedTask;
        }

        membership.UpdateRole(role);
        return Task.CompletedTask;
    }

    public Task<bool> HasResidentMembershipAsync(Guid userId, CancellationToken cancellationToken)
    {
        var hasResident = _dataStore.Memberships.Any(m =>
            m.UserId == userId &&
            m.Role == MembershipRole.Resident);

        return Task.FromResult(hasResident);
    }

    public Task<TerritoryMembership?> GetResidentMembershipAsync(Guid userId, CancellationToken cancellationToken)
    {
        var membership = _dataStore.Memberships.FirstOrDefault(m =>
            m.UserId == userId &&
            m.Role == MembershipRole.Resident);

        return Task.FromResult(membership);
    }

    public Task<IReadOnlyList<TerritoryMembership>> ListByUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var memberships = _dataStore.Memberships
            .Where(m => m.UserId == userId)
            .ToList();

        return Task.FromResult<IReadOnlyList<TerritoryMembership>>(memberships);
    }

    public Task UpdateResidencyVerificationAsync(Guid membershipId, ResidencyVerification verification, CancellationToken cancellationToken)
    {
        var membership = _dataStore.Memberships.FirstOrDefault(m => m.Id == membershipId);
        if (membership is null)
        {
            return Task.CompletedTask;
        }

        membership.UpdateResidencyVerification(verification);
        return Task.CompletedTask;
    }

    public Task UpdateGeoVerificationAsync(Guid membershipId, DateTime verifiedAtUtc, CancellationToken cancellationToken)
    {
        var membership = _dataStore.Memberships.FirstOrDefault(m => m.Id == membershipId);
        if (membership is null)
        {
            return Task.CompletedTask;
        }

        membership.UpdateGeoVerification(verifiedAtUtc);
        return Task.CompletedTask;
    }

    public Task UpdateDocumentVerificationAsync(Guid membershipId, DateTime verifiedAtUtc, CancellationToken cancellationToken)
    {
        var membership = _dataStore.Memberships.FirstOrDefault(m => m.Id == membershipId);
        if (membership is null)
        {
            return Task.CompletedTask;
        }

        membership.UpdateDocumentVerification(verifiedAtUtc);
        return Task.CompletedTask;
    }
}
