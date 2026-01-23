using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Domain.Membership;
using Araponga.Domain.Policies;
using Araponga.Domain.Users;

namespace Araponga.Application.Services;

public sealed class AccessEvaluator
{
    private readonly ITerritoryMembershipRepository _membershipRepository;
    private readonly IMembershipCapabilityRepository _capabilityRepository;
    private readonly ISystemPermissionRepository _systemPermissionRepository;
    private readonly MembershipAccessRules _accessRules;
    private readonly IDistributedCacheService _cache;
    private readonly CacheMetricsService? _metrics;
    private readonly PolicyRequirementService? _policyRequirementService;
    private readonly TermsAcceptanceService? _termsAcceptanceService;
    private readonly PrivacyPolicyAcceptanceService? _privacyAcceptanceService;

    public AccessEvaluator(
        ITerritoryMembershipRepository membershipRepository,
        IMembershipCapabilityRepository capabilityRepository,
        ISystemPermissionRepository systemPermissionRepository,
        MembershipAccessRules accessRules,
        IDistributedCacheService cache,
        CacheMetricsService? metrics = null,
        PolicyRequirementService? policyRequirementService = null,
        TermsAcceptanceService? termsAcceptanceService = null,
        PrivacyPolicyAcceptanceService? privacyAcceptanceService = null)
    {
        _membershipRepository = membershipRepository;
        _capabilityRepository = capabilityRepository;
        _systemPermissionRepository = systemPermissionRepository;
        _accessRules = accessRules;
        _cache = cache;
        _metrics = metrics;
        _policyRequirementService = policyRequirementService;
        _termsAcceptanceService = termsAcceptanceService;
        _privacyAcceptanceService = privacyAcceptanceService;
    }

    /// <summary>
    /// Verifica se o usuário é Resident validado no território.
    /// Usa ResidencyVerification para verificar status de verificação.
    /// </summary>
    public async Task<bool> IsResidentAsync(Guid userId, Guid territoryId, CancellationToken cancellationToken)
    {
        try
        {
            if (_cache is null)
            {
                // Se não há cache, verificar diretamente
                return await _accessRules.IsVerifiedResidentAsync(userId, territoryId, cancellationToken);
            }

            var cacheKey = $"membership:resident:{userId}:{territoryId}";
            var cached = await _cache.GetAsync<bool?>(cacheKey, cancellationToken);
            if (cached.HasValue)
            {
                _metrics?.RecordCacheAccess(cacheKey, hit: true);
                return cached.Value;
            }

            _metrics?.RecordCacheAccess(cacheKey, hit: false);

            var isVerifiedResident = await _accessRules.IsVerifiedResidentAsync(userId, territoryId, cancellationToken);

            await _cache.SetAsync(cacheKey, (bool?)isVerifiedResident, Constants.Cache.MembershipExpiration, cancellationToken);
            return isVerifiedResident;
        }
        catch
        {
            // Em caso de erro, verificar diretamente sem cache
            return await _accessRules.IsVerifiedResidentAsync(userId, territoryId, cancellationToken);
        }
    }

    /// <summary>
    /// Verifica se o usuário é Resident (pode ser não verificado ainda).
    /// </summary>
    public async Task<bool> IsResidentUnverifiedAsync(Guid userId, Guid territoryId, CancellationToken cancellationToken)
    {
        var membership = await _membershipRepository.GetByUserAndTerritoryAsync(userId, territoryId, cancellationToken);
        return membership is not null && membership.Role == MembershipRole.Resident;
    }

    public async Task<MembershipRole?> GetRoleAsync(Guid userId, Guid territoryId, CancellationToken cancellationToken)
    {
        var cacheKey = $"membership:role:{userId}:{territoryId}";
        var cached = await _cache.GetAsync<MembershipRole?>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            _metrics?.RecordCacheAccess(cacheKey, hit: true);
            return cached;
        }

        _metrics?.RecordCacheAccess(cacheKey, hit: false);

        var membership = await _membershipRepository.GetByUserAndTerritoryAsync(userId, territoryId, cancellationToken);
        var role = membership?.Role;

        await _cache.SetAsync(cacheKey, role, Constants.Cache.MembershipExpiration, cancellationToken);
        return role;
    }

    /// <summary>
    /// Verifica se o usuário tem capacidade no território.
    /// SystemAdmin tem implicitamente todas as capabilities em todos os territórios.
    /// Baseado em MembershipCapability para usuários não-admin.
    /// </summary>
    public async Task<bool> HasCapabilityAsync(
        Guid userId,
        Guid territoryId,
        MembershipCapabilityType capabilityType,
        CancellationToken cancellationToken)
    {
        try
        {
            // SystemAdmin tem implicitamente todas as capabilities em todos os territórios
            var isSystemAdmin = await IsSystemAdminAsync(userId, cancellationToken);
            if (isSystemAdmin)
            {
                return true;
            }

            var membership = await _membershipRepository.GetByUserAndTerritoryAsync(userId, territoryId, cancellationToken);
            if (membership is null)
            {
                return false;
            }

            return await _capabilityRepository.HasCapabilityAsync(membership.Id, capabilityType, cancellationToken);
        }
        catch
        {
            // Em caso de erro, retornar false (mais seguro)
            return false;
        }
    }

    /// <summary>
    /// Verifica se o usuário tem capacidade no território.
    /// SystemAdmin tem implicitamente todas as capabilities em todos os territórios.
    /// Versão que aceita Membership já carregado.
    /// </summary>
    public async Task<bool> HasCapabilityAsync(
        TerritoryMembership membership,
        MembershipCapabilityType capabilityType,
        CancellationToken cancellationToken)
    {
        // SystemAdmin tem implicitamente todas as capabilities em todos os territórios
        var isSystemAdmin = await IsSystemAdminAsync(membership.UserId, cancellationToken);
        if (isSystemAdmin)
        {
            return true;
        }

        return await _capabilityRepository.HasCapabilityAsync(membership.Id, capabilityType, cancellationToken);
    }

    /// <summary>
    /// Verifica se o usuário tem uma permissão global do sistema.
    /// </summary>
    public async Task<bool> HasSystemPermissionAsync(
        Guid userId,
        SystemPermissionType permissionType,
        CancellationToken cancellationToken)
    {
        try
        {
            if (_cache is null)
            {
                // Se não há cache, verificar diretamente no repositório
                if (_systemPermissionRepository is null)
                {
                    return false;
                }
                return await _systemPermissionRepository
                    .HasActivePermissionAsync(userId, permissionType, cancellationToken);
            }

            var cacheKey = $"system:permission:{userId}:{permissionType}";
            var cached = await _cache.GetAsync<bool?>(cacheKey, cancellationToken);
            if (cached.HasValue)
            {
                _metrics?.RecordCacheAccess(cacheKey, hit: true);
                return cached.Value;
            }

            _metrics?.RecordCacheAccess(cacheKey, hit: false);

            if (_systemPermissionRepository is null)
            {
                return false;
            }

            var hasPermission = await _systemPermissionRepository
                .HasActivePermissionAsync(userId, permissionType, cancellationToken);

            await _cache.SetAsync(cacheKey, (bool?)hasPermission, Constants.Cache.SystemPermissionExpiration, cancellationToken);
            return hasPermission;
        }
        catch
        {
            // Em caso de erro, retornar false (mais seguro)
            return false;
        }
    }

    /// <summary>
    /// Verifica se o usuário é administrador do sistema.
    /// </summary>
    public async Task<bool> IsSystemAdminAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await HasSystemPermissionAsync(
            userId,
            SystemPermissionType.SystemAdmin,
            cancellationToken);
    }

    /// <summary>
    /// [Obsoleto] Use HasCapabilityAsync ao invés disso.
    /// Mantido temporariamente para compatibilidade durante migração.
    /// </summary>
    [Obsolete("Use HasCapabilityAsync instead")]
    public bool IsCurator(User user)
    {
        // Este método não pode mais funcionar sem UserRole
        // Mantido apenas para evitar quebrar código que ainda o chama
        return false;
    }

    /// <summary>
    /// Invalidates membership cache for a user-territory pair.
    /// </summary>
    public async Task InvalidateMembershipCacheAsync(Guid userId, Guid territoryId, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync($"membership:resident:{userId}:{territoryId}", cancellationToken);
        await _cache.RemoveAsync($"membership:role:{userId}:{territoryId}", cancellationToken);
    }

    /// <summary>
    /// Invalidates membership cache for a user-territory pair (synchronous version for backward compatibility).
    /// </summary>
    public void InvalidateMembershipCache(Guid userId, Guid territoryId)
    {
        InvalidateMembershipCacheAsync(userId, territoryId).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Invalidates system permission cache for a user.
    /// </summary>
    public async Task InvalidateSystemPermissionCacheAsync(Guid userId, SystemPermissionType? permissionType = null, CancellationToken cancellationToken = default)
    {
        if (permissionType.HasValue)
        {
            await _cache.RemoveAsync($"system:permission:{userId}:{permissionType.Value}", cancellationToken);
        }
        else
        {
            // Remove todas as permissões do usuário
            foreach (SystemPermissionType type in Enum.GetValues(typeof(SystemPermissionType)))
            {
                await _cache.RemoveAsync($"system:permission:{userId}:{type}", cancellationToken);
            }
        }
    }

    /// <summary>
    /// Invalidates system permission cache for a user (synchronous version for backward compatibility).
    /// </summary>
    public void InvalidateSystemPermissionCache(Guid userId, SystemPermissionType? permissionType = null)
    {
        InvalidateSystemPermissionCacheAsync(userId, permissionType).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Verifica se o usuário aceitou todos os termos e políticas obrigatórios.
    /// </summary>
    public async Task<Result<bool>> HasAcceptedRequiredPoliciesAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        if (_policyRequirementService is null || _termsAcceptanceService is null || _privacyAcceptanceService is null)
        {
            // Se os serviços não estiverem disponíveis, retornar sucesso (não bloquear)
            return Result<bool>.Success(true);
        }

        var requirements = await _policyRequirementService.GetRequiredPoliciesForUserAsync(userId, cancellationToken);

        // Verificar aceite de termos
        var termsResult = await _termsAcceptanceService.HasAcceptedRequiredTermsAsync(
            userId,
            requirements.RequiredTerms,
            cancellationToken);

        if (termsResult.IsFailure)
        {
            return Result<bool>.Failure(termsResult.Error ?? "Failed to check terms acceptance.");
        }

        if (!termsResult.Value)
        {
            return Result<bool>.Success(false);
        }

        // Verificar aceite de políticas de privacidade
        var privacyResult = await _privacyAcceptanceService.HasAcceptedRequiredPoliciesAsync(
            userId,
            requirements.RequiredPrivacyPolicies,
            cancellationToken);

        if (privacyResult.IsFailure)
        {
            return Result<bool>.Failure(privacyResult.Error ?? "Failed to check privacy policy acceptance.");
        }

        return Result<bool>.Success(privacyResult.Value);
    }

    /// <summary>
    /// Obtém os termos e políticas que o usuário ainda precisa aceitar.
    /// </summary>
    public async Task<PolicyRequirements?> GetPendingPoliciesAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        if (_policyRequirementService is null || _termsAcceptanceService is null || _privacyAcceptanceService is null)
        {
            return null;
        }

        var requirements = await _policyRequirementService.GetRequiredPoliciesForUserAsync(userId, cancellationToken);

        // Filtrar termos não aceitos
        var pendingTerms = new List<TermsOfService>();
        foreach (var terms in requirements.RequiredTerms)
        {
            var hasAccepted = await _termsAcceptanceService.HasAcceptedTermsAsync(userId, terms.Id, cancellationToken);
            if (!hasAccepted)
            {
                pendingTerms.Add(terms);
            }
        }

        // Filtrar políticas não aceitas
        var pendingPolicies = new List<PrivacyPolicy>();
        foreach (var policy in requirements.RequiredPrivacyPolicies)
        {
            var hasAccepted = await _privacyAcceptanceService.HasAcceptedPolicyAsync(userId, policy.Id, cancellationToken);
            if (!hasAccepted)
            {
                pendingPolicies.Add(policy);
            }
        }

        return new PolicyRequirements(pendingTerms, pendingPolicies);
    }
}
