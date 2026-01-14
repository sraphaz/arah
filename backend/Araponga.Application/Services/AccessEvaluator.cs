using Araponga.Application.Interfaces;
using Araponga.Domain.Social;
using Araponga.Domain.Users;
using Microsoft.Extensions.Caching.Memory;

namespace Araponga.Application.Services;

public sealed class AccessEvaluator
{
    private readonly ITerritoryMembershipRepository _membershipRepository;
    private readonly MembershipAccessRules _accessRules;
    private readonly IMemoryCache _cache;
    private static readonly TimeSpan MembershipCacheExpiration = TimeSpan.FromMinutes(10);

    public AccessEvaluator(
        ITerritoryMembershipRepository membershipRepository,
        MembershipAccessRules accessRules,
        IMemoryCache cache)
    {
        _membershipRepository = membershipRepository;
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

        _cache.Set(cacheKey, isVerifiedResident, MembershipCacheExpiration);
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

    /// <summary>
    /// [Obsolete] Usa VerificationStatus legado. Use IsResidentAsync que usa ResidencyVerification.
    /// </summary>
    [Obsolete("Use IsResidentAsync which uses ResidencyVerification.")]
    public async Task<bool> IsResidentLegacyAsync(Guid userId, Guid territoryId, CancellationToken cancellationToken)
    {
        var cacheKey = $"membership:resident:legacy:{userId}:{territoryId}";
        if (_cache.TryGetValue<bool?>(cacheKey, out var cached))
        {
            return cached ?? false;
        }

        var membership = await _membershipRepository.GetByUserAndTerritoryAsync(userId, territoryId, cancellationToken);
        var isResident = membership is not null &&
                         membership.Role == MembershipRole.Resident &&
                         membership.VerificationStatus == VerificationStatus.Validated;

        _cache.Set(cacheKey, isResident, MembershipCacheExpiration);
        return isResident;
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

        _cache.Set(cacheKey, role, MembershipCacheExpiration);
        return role;
    }

    public bool IsCurator(User user)
    {
        return user.Role == UserRole.Curator;
    }

    /// <summary>
    /// Invalidates membership cache for a user-territory pair.
    /// </summary>
    public void InvalidateMembershipCache(Guid userId, Guid territoryId)
    {
        _cache.Remove($"membership:resident:{userId}:{territoryId}");
        _cache.Remove($"membership:role:{userId}:{territoryId}");
    }
}
