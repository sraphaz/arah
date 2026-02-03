using Araponga.Application.Interfaces;
using Araponga.Domain.Membership;
using Araponga.Domain.Policies;
using Araponga.Domain.Users;

namespace Araponga.Application.Services;

/// <summary>
/// Serviço para determinar políticas obrigatórias baseado em papéis, capabilities e system permissions.
/// </summary>
public sealed class PolicyRequirementService
{
    private readonly TermsOfServiceService _termsService;
    private readonly PrivacyPolicyService _privacyService;
    private readonly ITerritoryMembershipRepository _membershipRepository;
    private readonly IMembershipCapabilityRepository _capabilityRepository;
    private readonly ISystemPermissionRepository _systemPermissionRepository;

    public PolicyRequirementService(
        TermsOfServiceService termsService,
        PrivacyPolicyService privacyService,
        ITerritoryMembershipRepository membershipRepository,
        IMembershipCapabilityRepository capabilityRepository,
        ISystemPermissionRepository systemPermissionRepository)
    {
        _termsService = termsService;
        _privacyService = privacyService;
        _membershipRepository = membershipRepository;
        _capabilityRepository = capabilityRepository;
        _systemPermissionRepository = systemPermissionRepository;
    }

    /// <summary>
    /// Obtém todas as políticas obrigatórias para um usuário.
    /// </summary>
    public async Task<PolicyRequirements> GetRequiredPoliciesForUserAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        // Obter todos os memberships do usuário
        var memberships = await _membershipRepository.ListByUserAsync(userId, cancellationToken);
        var roles = memberships.Select(m => m.Role).Distinct().ToList();

        // Obter todas as capabilities do usuário através dos memberships
        var capabilityTypes = new List<MembershipCapabilityType>();
        foreach (var membership in memberships)
        {
            var capabilities = await _capabilityRepository.GetByMembershipIdAsync(membership.Id, cancellationToken);
            capabilityTypes.AddRange(capabilities.Select(c => c.CapabilityType));
        }
        capabilityTypes = capabilityTypes.Distinct().ToList();

        // Obter todas as system permissions do usuário
        var systemPermissions = await _systemPermissionRepository.GetByUserIdAsync(userId, cancellationToken);
        var permissionTypes = systemPermissions.Select(p => p.PermissionType).Distinct().ToList();

        // Obter termos obrigatórios
        var requiredTerms = await _termsService.GetRequiredTermsForUserAsync(
            userId,
            roles,
            capabilityTypes,
            permissionTypes,
            cancellationToken);

        // Obter políticas de privacidade obrigatórias
        var requiredPrivacyPolicies = await _privacyService.GetRequiredPoliciesForUserAsync(
            userId,
            roles,
            capabilityTypes,
            permissionTypes,
            cancellationToken);

        return new PolicyRequirements(requiredTerms, requiredPrivacyPolicies);
    }

    /// <summary>
    /// Obtém políticas obrigatórias para um papel específico.
    /// </summary>
    public async Task<PolicyRequirements> GetRequiredPoliciesForRoleAsync(
        MembershipRole role,
        CancellationToken cancellationToken)
    {
        var requiredTerms = await _termsService.GetRequiredTermsForRoleAsync(role, cancellationToken);
        var requiredPrivacyPolicies = await _privacyService.GetRequiredPoliciesForRoleAsync(role, cancellationToken);

        return new PolicyRequirements(requiredTerms, requiredPrivacyPolicies);
    }

    /// <summary>
    /// Obtém políticas obrigatórias para uma capability específica.
    /// </summary>
    public async Task<PolicyRequirements> GetRequiredPoliciesForCapabilityAsync(
        MembershipCapabilityType capability,
        CancellationToken cancellationToken)
    {
        var requiredTerms = await _termsService.GetRequiredTermsForCapabilityAsync(capability, cancellationToken);
        var requiredPrivacyPolicies = await _privacyService.GetRequiredPoliciesForCapabilityAsync(capability, cancellationToken);

        return new PolicyRequirements(requiredTerms, requiredPrivacyPolicies);
    }

    /// <summary>
    /// Obtém políticas obrigatórias para uma system permission específica.
    /// </summary>
    public async Task<PolicyRequirements> GetRequiredPoliciesForSystemPermissionAsync(
        SystemPermissionType permission,
        CancellationToken cancellationToken)
    {
        var requiredTerms = await _termsService.GetRequiredTermsForSystemPermissionAsync(permission, cancellationToken);
        var requiredPrivacyPolicies = await _privacyService.GetRequiredPoliciesForSystemPermissionAsync(permission, cancellationToken);

        return new PolicyRequirements(requiredTerms, requiredPrivacyPolicies);
    }

    /// <summary>
    /// Obtém políticas obrigatórias para uma ação específica (ex: criar evento, criar loja, moderar).
    /// </summary>
    public async Task<PolicyRequirements> GetRequiredPoliciesForActionAsync(
        string action,
        Guid userId,
        CancellationToken cancellationToken)
    {
        // Mapear ações para políticas específicas
        // Por enquanto, retornar todas as políticas obrigatórias do usuário
        // Pode ser estendido para mapear ações específicas para políticas específicas
        return await GetRequiredPoliciesForUserAsync(userId, cancellationToken);
    }
}

/// <summary>
/// Representa os requisitos de políticas para um usuário ou contexto.
/// </summary>
public sealed class PolicyRequirements
{
    public IReadOnlyList<TermsOfService> RequiredTerms { get; }
    public IReadOnlyList<PrivacyPolicy> RequiredPrivacyPolicies { get; }

    public PolicyRequirements(
        IReadOnlyList<TermsOfService> requiredTerms,
        IReadOnlyList<PrivacyPolicy> requiredPrivacyPolicies)
    {
        RequiredTerms = requiredTerms;
        RequiredPrivacyPolicies = requiredPrivacyPolicies;
    }

    public bool IsEmpty => RequiredTerms.Count == 0 && RequiredPrivacyPolicies.Count == 0;
}
