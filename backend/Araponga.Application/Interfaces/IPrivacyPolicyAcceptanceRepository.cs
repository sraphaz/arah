using Araponga.Domain.Policies;

namespace Araponga.Application.Interfaces;

/// <summary>
/// Repositório para gerenciar aceites de Políticas de Privacidade.
/// </summary>
public interface IPrivacyPolicyAcceptanceRepository
{
    /// <summary>
    /// Obtém aceite por ID.
    /// </summary>
    Task<PrivacyPolicyAcceptance?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Verifica se o usuário aceitou a política específica.
    /// </summary>
    Task<PrivacyPolicyAcceptance?> GetByUserAndPolicyAsync(Guid userId, Guid policyId, CancellationToken cancellationToken);

    /// <summary>
    /// Obtém todos os aceites de um usuário.
    /// </summary>
    Task<IReadOnlyList<PrivacyPolicyAcceptance>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Obtém todos os aceites de uma política específica.
    /// </summary>
    Task<IReadOnlyList<PrivacyPolicyAcceptance>> GetByPolicyIdAsync(Guid policyId, CancellationToken cancellationToken);

    /// <summary>
    /// Verifica se o usuário aceitou a política específica e não revogou.
    /// </summary>
    Task<bool> HasAcceptedAsync(Guid userId, Guid policyId, CancellationToken cancellationToken);

    /// <summary>
    /// Adiciona um novo aceite.
    /// </summary>
    Task AddAsync(PrivacyPolicyAcceptance acceptance, CancellationToken cancellationToken);

    /// <summary>
    /// Atualiza um aceite existente.
    /// </summary>
    Task UpdateAsync(PrivacyPolicyAcceptance acceptance, CancellationToken cancellationToken);
}
