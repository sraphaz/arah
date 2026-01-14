using Araponga.Application.Interfaces;
using Araponga.Domain.Social;
using Araponga.Domain.Users;

namespace Araponga.Application.Services;

/// <summary>
/// Centraliza regras de acesso relacionadas a Membership.
/// Separa lógica de permissões do AccessEvaluator para facilitar manutenção.
/// </summary>
public sealed class MembershipAccessRules
{
    private readonly ITerritoryMembershipRepository _membershipRepository;
    private readonly IUserRepository _userRepository;

    public MembershipAccessRules(
        ITerritoryMembershipRepository membershipRepository,
        IUserRepository userRepository)
    {
        _membershipRepository = membershipRepository;
        _userRepository = userRepository;
    }

    /// <summary>
    /// Verifica se o usuário pode criar uma Store no território.
    /// Regra: Role = Resident && ResidencyVerification != Unverified
    /// </summary>
    public async Task<bool> CanCreateStoreAsync(Guid userId, Guid territoryId, CancellationToken cancellationToken)
    {
        var membership = await _membershipRepository.GetByUserAndTerritoryAsync(userId, territoryId, cancellationToken);
        if (membership is null)
        {
            return false;
        }

        return membership.Role == MembershipRole.Resident &&
               membership.ResidencyVerification != ResidencyVerification.Unverified;
    }

    /// <summary>
    /// Verifica se o usuário pode criar um Item (produto/serviço) no território.
    /// Regra: Role = Resident && ResidencyVerification != Unverified
    /// </summary>
    public async Task<bool> CanCreateItemAsync(Guid userId, Guid territoryId, CancellationToken cancellationToken)
    {
        return await CanCreateStoreAsync(userId, territoryId, cancellationToken);
    }

    /// <summary>
    /// Verifica se o usuário pode publicar um Item no marketplace.
    /// Regra: Role = Resident && ResidencyVerification != Unverified && MarketplaceIdentityVerifiedAtUtc != null
    /// Nota: MarketplaceIdentityVerifiedAtUtc ainda não existe no User, será implementado futuramente.
    /// </summary>
    public async Task<bool> CanPublishItemAsync(Guid userId, Guid territoryId, CancellationToken cancellationToken)
    {
        // Primeiro verifica se pode criar item
        if (!await CanCreateItemAsync(userId, territoryId, cancellationToken))
        {
            return false;
        }

        // TODO: Adicionar verificação de MarketplaceIdentityVerifiedAtUtc quando implementado
        // Esta verificação será necessária para habilitar operações econômicas no marketplace.
        // O campo MarketplaceIdentityVerifiedAtUtc deve ser adicionado ao User quando a funcionalidade
        // de verificação de identidade para marketplace for implementada.
        // var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        // if (user is null)
        // {
        //     return false;
        // }
        // return user.MarketplaceIdentityVerifiedAtUtc != null;
        
        // Por enquanto, retorna true se pode criar item (sem verificação de identidade marketplace)
        return true;
    }

    /// <summary>
    /// Verifica se o usuário é Resident validado no território.
    /// Regra: Role = Resident && ResidencyVerification != Unverified
    /// </summary>
    public async Task<bool> IsVerifiedResidentAsync(Guid userId, Guid territoryId, CancellationToken cancellationToken)
    {
        return await CanCreateStoreAsync(userId, territoryId, cancellationToken);
    }
}
