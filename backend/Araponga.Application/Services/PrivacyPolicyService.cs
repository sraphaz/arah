using Araponga.Application.Interfaces;
using Araponga.Domain.Membership;
using Araponga.Domain.Policies;
using Araponga.Domain.Users;
using System.Text.Json;

namespace Araponga.Application.Services;

/// <summary>
/// Serviço para gerenciar Políticas de Privacidade.
/// </summary>
public sealed class PrivacyPolicyService
{
    private readonly IPrivacyPolicyRepository _repository;

    public PrivacyPolicyService(IPrivacyPolicyRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Obtém todas as políticas ativas.
    /// </summary>
    public Task<IReadOnlyList<PrivacyPolicy>> GetActivePoliciesAsync(CancellationToken cancellationToken)
    {
        return _repository.GetActiveAsync(cancellationToken);
    }

    /// <summary>
    /// Obtém política por versão.
    /// </summary>
    public Task<PrivacyPolicy?> GetPolicyByVersionAsync(string version, CancellationToken cancellationToken)
    {
        return _repository.GetByVersionAsync(version, cancellationToken);
    }

    /// <summary>
    /// Obtém política por ID.
    /// </summary>
    public Task<PrivacyPolicy?> GetPolicyByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _repository.GetByIdAsync(id, cancellationToken);
    }

    /// <summary>
    /// Obtém políticas obrigatórias para um papel específico.
    /// </summary>
    public async Task<IReadOnlyList<PrivacyPolicy>> GetRequiredPoliciesForRoleAsync(
        MembershipRole role,
        CancellationToken cancellationToken)
    {
        var allPolicies = await _repository.GetActiveAsync(cancellationToken);
        return allPolicies
            .Where(policy => IsRequiredForRole(policy, role))
            .ToList();
    }

    /// <summary>
    /// Obtém políticas obrigatórias para uma capability específica.
    /// </summary>
    public async Task<IReadOnlyList<PrivacyPolicy>> GetRequiredPoliciesForCapabilityAsync(
        MembershipCapabilityType capability,
        CancellationToken cancellationToken)
    {
        var allPolicies = await _repository.GetActiveAsync(cancellationToken);
        return allPolicies
            .Where(policy => IsRequiredForCapability(policy, capability))
            .ToList();
    }

    /// <summary>
    /// Obtém políticas obrigatórias para uma system permission específica.
    /// </summary>
    public async Task<IReadOnlyList<PrivacyPolicy>> GetRequiredPoliciesForSystemPermissionAsync(
        SystemPermissionType permission,
        CancellationToken cancellationToken)
    {
        var allPolicies = await _repository.GetActiveAsync(cancellationToken);
        return allPolicies
            .Where(policy => IsRequiredForSystemPermission(policy, permission))
            .ToList();
    }

    /// <summary>
    /// Obtém todas as políticas obrigatórias para um usuário baseado em seus papéis, capabilities e permissions.
    /// </summary>
    public async Task<IReadOnlyList<PrivacyPolicy>> GetRequiredPoliciesForUserAsync(
        Guid userId,
        IReadOnlyList<MembershipRole> roles,
        IReadOnlyList<MembershipCapabilityType> capabilities,
        IReadOnlyList<SystemPermissionType> systemPermissions,
        CancellationToken cancellationToken)
    {
        var allPolicies = await _repository.GetActiveAsync(cancellationToken);
        var requiredPolicies = new HashSet<PrivacyPolicy>();

        foreach (var role in roles)
        {
            var rolePolicies = allPolicies.Where(policy => IsRequiredForRole(policy, role));
            foreach (var policy in rolePolicies)
            {
                requiredPolicies.Add(policy);
            }
        }

        foreach (var capability in capabilities)
        {
            var capabilityPolicies = allPolicies.Where(policy => IsRequiredForCapability(policy, capability));
            foreach (var policy in capabilityPolicies)
            {
                requiredPolicies.Add(policy);
            }
        }

        foreach (var permission in systemPermissions)
        {
            var permissionPolicies = allPolicies.Where(policy => IsRequiredForSystemPermission(policy, permission));
            foreach (var policy in permissionPolicies)
            {
                requiredPolicies.Add(policy);
            }
        }

        return requiredPolicies.ToList();
    }

    private static bool IsRequiredForRole(PrivacyPolicy policy, MembershipRole role)
    {
        if (string.IsNullOrWhiteSpace(policy.RequiredRoles))
        {
            return false;
        }

        try
        {
            var roles = JsonSerializer.Deserialize<List<int>>(policy.RequiredRoles);
            return roles?.Contains((int)role) ?? false;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsRequiredForCapability(PrivacyPolicy policy, MembershipCapabilityType capability)
    {
        if (string.IsNullOrWhiteSpace(policy.RequiredCapabilities))
        {
            return false;
        }

        try
        {
            var capabilities = JsonSerializer.Deserialize<List<int>>(policy.RequiredCapabilities);
            return capabilities?.Contains((int)capability) ?? false;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsRequiredForSystemPermission(PrivacyPolicy policy, SystemPermissionType permission)
    {
        if (string.IsNullOrWhiteSpace(policy.RequiredSystemPermissions))
        {
            return false;
        }

        try
        {
            var permissions = JsonSerializer.Deserialize<List<int>>(policy.RequiredSystemPermissions);
            return permissions?.Contains((int)permission) ?? false;
        }
        catch
        {
            return false;
        }
    }
}
