using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Domain.Membership;
using Araponga.Domain.Users;

namespace Araponga.Application.Services;

/// <summary>
/// Centraliza regras de acesso relacionadas a Membership.
/// Separa lógica de permissões do AccessEvaluator para facilitar manutenção.
/// </summary>
public sealed class MembershipAccessRules
{
    private readonly ITerritoryMembershipRepository _membershipRepository;
    private readonly IMembershipSettingsRepository _settingsRepository;
    private readonly IUserRepository _userRepository;
    private readonly IFeatureFlagService _featureFlagService;

    public MembershipAccessRules(
        ITerritoryMembershipRepository membershipRepository,
        IMembershipSettingsRepository settingsRepository,
        IUserRepository userRepository,
        IFeatureFlagService featureFlagService)
    {
        _membershipRepository = membershipRepository;
        _settingsRepository = settingsRepository;
        _userRepository = userRepository;
        _featureFlagService = featureFlagService;
    }

    /// <summary>
    /// Verifica se o usuário pode criar uma Store no território.
    /// Regra: Role = Resident && HasAnyVerification()
    /// </summary>
    public async Task<bool> CanCreateStoreAsync(Guid userId, Guid territoryId, CancellationToken cancellationToken)
    {
        var membership = await _membershipRepository.GetByUserAndTerritoryAsync(userId, territoryId, cancellationToken);
        if (membership is null)
        {
            return false;
        }

        return membership.Role == MembershipRole.Resident &&
               membership.HasAnyVerification();
    }

    /// <summary>
    /// Verifica se o usuário pode criar um Item (produto/serviço) no território.
    /// Regra: Role = Resident && HasAnyVerification()
    /// </summary>
    public async Task<bool> CanCreateItemAsync(Guid userId, Guid territoryId, CancellationToken cancellationToken)
    {
        return await CanCreateStoreAsync(userId, territoryId, cancellationToken);
    }

    /// <summary>
    /// Verifica se o usuário pode criar Store/Item no marketplace.
    /// Regras compostas:
    /// 1. Territory.FeatureFlags.MarketplaceEnabled == true
    /// 2. MembershipSettings.MarketplaceOptIn == true
    /// 3. Membership.Role == Resident
    /// 4. Membership.HasAnyVerification()
    /// </summary>
    public async Task<bool> CanCreateStoreOrItemInMarketplaceAsync(Guid userId, Guid territoryId, CancellationToken cancellationToken)
    {
        // 1. Feature flag (mais barato, cacheável)
        if (!_featureFlagService.IsEnabled(territoryId, FeatureFlag.MarketplaceEnabled))
        {
            return false;
        }

        // 2. Membership (já carregado na maioria dos casos)
        var membership = await _membershipRepository.GetByUserAndTerritoryAsync(userId, territoryId, cancellationToken);
        if (membership is null || membership.Role != MembershipRole.Resident)
        {
            return false;
        }

        if (!membership.HasAnyVerification())
        {
            return false;
        }

        // 3. Settings (requer join)
        var settings = await _settingsRepository.GetByMembershipIdAsync(membership.Id, cancellationToken);
        if (settings?.MarketplaceOptIn != true)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Verifica se o usuário pode operar plenamente no marketplace (publicar / responder interesse).
    /// Requer tudo acima + User.IdentityVerificationStatus == Verified
    /// </summary>
    public async Task<bool> CanPublishItemAsync(Guid userId, Guid territoryId, CancellationToken cancellationToken)
    {
        // Primeiro verifica se pode criar item
        if (!await CanCreateStoreOrItemInMarketplaceAsync(userId, territoryId, cancellationToken))
        {
            return false;
        }

        // 4. User identity verification (requer join)
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return false;
        }

        return user.IdentityVerificationStatus == UserIdentityVerificationStatus.Verified;
    }

    /// <summary>
    /// Verifica se o usuário é Resident validado no território.
    /// Regra: Role = Resident && HasAnyVerification()
    /// </summary>
    public async Task<bool> IsVerifiedResidentAsync(Guid userId, Guid territoryId, CancellationToken cancellationToken)
    {
        return await CanCreateStoreAsync(userId, territoryId, cancellationToken);
    }
}
