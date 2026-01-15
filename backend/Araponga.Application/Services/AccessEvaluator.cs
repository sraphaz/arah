using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Domain.Membership;
using Araponga.Domain.Users;
using Microsoft.Extensions.Caching.Memory;

namespace Araponga.Application.Services;

public sealed class AccessEvaluator
{
    private readonly ITerritoryMembershipRepository _membershipRepository;
    private readonly IMembershipCapabilityRepository _capabilityRepository;
    private readonly ISystemPermissionRepository _systemPermissionRepository;
    private readonly MembershipAccessRules _accessRules;
    private readonly IMemoryCache _cache;

    public AccessEvaluator(
        ITerritoryMembershipRepository membershipRepository,
        IMembershipCapabilityRepository capabilityRepository,
        ISystemPermissionRepository systemPermissionRepository,
        MembershipAccessRules accessRules,
        IMemoryCache cache)
    {
        _membershipRepository = membershipRepository;
        _capabilityRepository = capabilityRepository;
        _systemPermissionRepository = systemPermissionRepository;
        _accessRules = accessRules;
        _cache = cache;
    }

    /// <summary>
    /// Verifica se o usuário é Resident validado no território.
    /// Usa ResidencyVerification para verificar status de verificação.
    /// </summary>
    public async Task<bool> IsResidentAsync(Guid userId, Guid territoryId, CancellationToken cancellationToken)
    {
        var cacheKey = $"membership:resident:{userId}:{territoryId}";
        if (_cache.TryGetValue<bool?>(cacheKey, out var cached))
        {
            return cached ?? false;
        }

        var isVerifiedResident = await _accessRules.IsVerifiedResidentAsync(userId, territoryId, cancellationToken);

        _cache.Set(cacheKey, isVerifiedResident, Constants.Cache.MembershipExpiration);
        return isVerifiedResident;
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
        if (_cache.TryGetValue<MembershipRole?>(cacheKey, out var cached))
        {
            return cached;
        }

        var membership = await _membershipRepository.GetByUserAndTerritoryAsync(userId, territoryId, cancellationToken);
        var role = membership?.Role;

        _cache.Set(cacheKey, role, Constants.Cache.MembershipExpiration);
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
        var cacheKey = $"system:permission:{userId}:{permissionType}";
        if (_cache.TryGetValue<bool?>(cacheKey, out var cached))
        {
            return cached ?? false;
        }

        var hasPermission = await _systemPermissionRepository
            .HasActivePermissionAsync(userId, permissionType, cancellationToken);

        _cache.Set(cacheKey, hasPermission, Constants.Cache.SystemPermissionExpiration);
        return hasPermission;
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
    public void InvalidateMembershipCache(Guid userId, Guid territoryId)
    {
        _cache.Remove($"membership:resident:{userId}:{territoryId}");
        _cache.Remove($"membership:role:{userId}:{territoryId}");
    }

    /// <summary>
    /// Invalidates system permission cache for a user.
    /// </summary>
    public void InvalidateSystemPermissionCache(Guid userId, SystemPermissionType? permissionType = null)
    {
        if (permissionType.HasValue)
        {
            _cache.Remove($"system:permission:{userId}:{permissionType.Value}");
        }
        else
        {
            // Remove todas as permissões do usuário
            foreach (SystemPermissionType type in Enum.GetValues(typeof(SystemPermissionType)))
            {
                _cache.Remove($"system:permission:{userId}:{type}");
            }
        }
    }
}
