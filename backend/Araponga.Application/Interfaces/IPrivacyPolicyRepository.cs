using Araponga.Domain.Policies;

namespace Araponga.Application.Interfaces;

/// <summary>
/// Repositório para gerenciar Políticas de Privacidade.
/// </summary>
public interface IPrivacyPolicyRepository
{
    /// <summary>
    /// Obtém política de privacidade por ID.
    /// </summary>
    Task<PrivacyPolicy?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Obtém política de privacidade por versão.
    /// </summary>
    Task<PrivacyPolicy?> GetByVersionAsync(string version, CancellationToken cancellationToken);

    /// <summary>
    /// Obtém todas as políticas de privacidade ativas.
    /// </summary>
    Task<IReadOnlyList<PrivacyPolicy>> GetActiveAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Lista todas as políticas de privacidade (ativas e inativas).
    /// </summary>
    Task<IReadOnlyList<PrivacyPolicy>> ListAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Adiciona nova política de privacidade.
    /// </summary>
    Task AddAsync(PrivacyPolicy policy, CancellationToken cancellationToken);

    /// <summary>
    /// Atualiza política de privacidade existente.
    /// </summary>
    Task UpdateAsync(PrivacyPolicy policy, CancellationToken cancellationToken);
}
