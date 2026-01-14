using Araponga.Domain.Social;

namespace Araponga.Application.Interfaces;

public interface ITerritoryMembershipRepository
{
    Task<TerritoryMembership?> GetByUserAndTerritoryAsync(Guid userId, Guid territoryId, CancellationToken cancellationToken);
    Task<TerritoryMembership?> GetByIdAsync(Guid membershipId, CancellationToken cancellationToken);
    Task<bool> HasValidatedResidentAsync(Guid territoryId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Guid>> ListResidentUserIdsAsync(Guid territoryId, CancellationToken cancellationToken);
    Task AddAsync(TerritoryMembership membership, CancellationToken cancellationToken);
    
    /// <summary>
    /// Atualiza o membership completo. Usa a entidade de domínio como fonte da verdade.
    /// </summary>
    Task UpdateAsync(TerritoryMembership membership, CancellationToken cancellationToken);
    
    /// <summary>
    /// Atualiza apenas o Role do membership, sem afetar ResidencyVerification.
    /// </summary>
    Task UpdateRoleAsync(Guid membershipId, MembershipRole role, CancellationToken cancellationToken);
    
    /// <summary>
    /// Verifica se o usuário já possui um Membership como Resident em qualquer território.
    /// Usado para garantir a regra "1 Resident por User".
    /// </summary>
    Task<bool> HasResidentMembershipAsync(Guid userId, CancellationToken cancellationToken);
    
    /// <summary>
    /// Obtém o Membership Resident do usuário, se existir.
    /// Usado para transferência de residência.
    /// </summary>
    Task<TerritoryMembership?> GetResidentMembershipAsync(Guid userId, CancellationToken cancellationToken);
    
    /// <summary>
    /// Lista todos os Memberships do usuário.
    /// </summary>
    Task<IReadOnlyList<TerritoryMembership>> ListByUserAsync(Guid userId, CancellationToken cancellationToken);
    
    /// <summary>
    /// Atualiza o ResidencyVerification do membership.
    /// </summary>
    Task UpdateResidencyVerificationAsync(Guid membershipId, ResidencyVerification verification, CancellationToken cancellationToken);
    
    /// <summary>
    /// Atualiza a verificação geo do membership.
    /// </summary>
    Task UpdateGeoVerificationAsync(Guid membershipId, DateTime verifiedAtUtc, CancellationToken cancellationToken);
    
    /// <summary>
    /// Atualiza a verificação documental do membership.
    /// </summary>
    Task UpdateDocumentVerificationAsync(Guid membershipId, DateTime verifiedAtUtc, CancellationToken cancellationToken);
}
