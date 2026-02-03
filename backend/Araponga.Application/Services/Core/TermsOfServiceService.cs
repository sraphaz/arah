using Araponga.Application.Interfaces;
using Araponga.Domain.Membership;
using Araponga.Domain.Policies;
using Araponga.Domain.Users;
using System.Text.Json;

namespace Araponga.Application.Services;

/// <summary>
/// Serviço para gerenciar Termos de Uso.
/// </summary>
public sealed class TermsOfServiceService
{
    private readonly ITermsOfServiceRepository _repository;

    public TermsOfServiceService(ITermsOfServiceRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Obtém todos os termos ativos.
    /// </summary>
    public Task<IReadOnlyList<TermsOfService>> GetActiveTermsAsync(CancellationToken cancellationToken)
    {
        return _repository.GetActiveAsync(cancellationToken);
    }

    /// <summary>
    /// Obtém termos por versão.
    /// </summary>
    public Task<TermsOfService?> GetTermsByVersionAsync(string version, CancellationToken cancellationToken)
    {
        return _repository.GetByVersionAsync(version, cancellationToken);
    }

    /// <summary>
    /// Obtém termos por ID.
    /// </summary>
    public Task<TermsOfService?> GetTermsByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _repository.GetByIdAsync(id, cancellationToken);
    }

    /// <summary>
    /// Obtém termos obrigatórios para um papel específico.
    /// </summary>
    public async Task<IReadOnlyList<TermsOfService>> GetRequiredTermsForRoleAsync(
        MembershipRole role,
        CancellationToken cancellationToken)
    {
        var allTerms = await _repository.GetActiveAsync(cancellationToken);
        return allTerms
            .Where(terms => IsRequiredForRole(terms, role))
            .ToList();
    }

    /// <summary>
    /// Obtém termos obrigatórios para uma capability específica.
    /// </summary>
    public async Task<IReadOnlyList<TermsOfService>> GetRequiredTermsForCapabilityAsync(
        MembershipCapabilityType capability,
        CancellationToken cancellationToken)
    {
        var allTerms = await _repository.GetActiveAsync(cancellationToken);
        return allTerms
            .Where(terms => IsRequiredForCapability(terms, capability))
            .ToList();
    }

    /// <summary>
    /// Obtém termos obrigatórios para uma system permission específica.
    /// </summary>
    public async Task<IReadOnlyList<TermsOfService>> GetRequiredTermsForSystemPermissionAsync(
        SystemPermissionType permission,
        CancellationToken cancellationToken)
    {
        var allTerms = await _repository.GetActiveAsync(cancellationToken);
        return allTerms
            .Where(terms => IsRequiredForSystemPermission(terms, permission))
            .ToList();
    }

    /// <summary>
    /// Obtém todos os termos obrigatórios para um usuário baseado em seus papéis, capabilities e permissions.
    /// </summary>
    public async Task<IReadOnlyList<TermsOfService>> GetRequiredTermsForUserAsync(
        Guid userId,
        IReadOnlyList<MembershipRole> roles,
        IReadOnlyList<MembershipCapabilityType> capabilities,
        IReadOnlyList<SystemPermissionType> systemPermissions,
        CancellationToken cancellationToken)
    {
        var allTerms = await _repository.GetActiveAsync(cancellationToken);
        var requiredTerms = new HashSet<TermsOfService>();

        foreach (var role in roles)
        {
            var roleTerms = allTerms.Where(terms => IsRequiredForRole(terms, role));
            foreach (var term in roleTerms)
            {
                requiredTerms.Add(term);
            }
        }

        foreach (var capability in capabilities)
        {
            var capabilityTerms = allTerms.Where(terms => IsRequiredForCapability(terms, capability));
            foreach (var term in capabilityTerms)
            {
                requiredTerms.Add(term);
            }
        }

        foreach (var permission in systemPermissions)
        {
            var permissionTerms = allTerms.Where(terms => IsRequiredForSystemPermission(terms, permission));
            foreach (var term in permissionTerms)
            {
                requiredTerms.Add(term);
            }
        }

        return requiredTerms.ToList();
    }

    private static bool IsRequiredForRole(TermsOfService terms, MembershipRole role)
    {
        if (string.IsNullOrWhiteSpace(terms.RequiredRoles))
        {
            return false;
        }

        try
        {
            var roles = JsonSerializer.Deserialize<List<int>>(terms.RequiredRoles);
            return roles?.Contains((int)role) ?? false;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsRequiredForCapability(TermsOfService terms, MembershipCapabilityType capability)
    {
        if (string.IsNullOrWhiteSpace(terms.RequiredCapabilities))
        {
            return false;
        }

        try
        {
            var capabilities = JsonSerializer.Deserialize<List<int>>(terms.RequiredCapabilities);
            return capabilities?.Contains((int)capability) ?? false;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsRequiredForSystemPermission(TermsOfService terms, SystemPermissionType permission)
    {
        if (string.IsNullOrWhiteSpace(terms.RequiredSystemPermissions))
        {
            return false;
        }

        try
        {
            var permissions = JsonSerializer.Deserialize<List<int>>(terms.RequiredSystemPermissions);
            return permissions?.Contains((int)permission) ?? false;
        }
        catch
        {
            return false;
        }
    }
}
