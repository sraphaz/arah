using Araponga.Domain.Policies;

namespace Araponga.Application.Interfaces;

/// <summary>
/// Repositório para gerenciar aceites de Termos de Uso.
/// </summary>
public interface ITermsAcceptanceRepository
{
    /// <summary>
    /// Obtém aceite por ID.
    /// </summary>
    Task<TermsAcceptance?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Verifica se o usuário aceitou os termos específicos.
    /// </summary>
    Task<TermsAcceptance?> GetByUserAndTermsAsync(Guid userId, Guid termsId, CancellationToken cancellationToken);

    /// <summary>
    /// Obtém todos os aceites de um usuário.
    /// </summary>
    Task<IReadOnlyList<TermsAcceptance>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Obtém todos os aceites de termos específicos.
    /// </summary>
    Task<IReadOnlyList<TermsAcceptance>> GetByTermsIdAsync(Guid termsId, CancellationToken cancellationToken);

    /// <summary>
    /// Verifica se o usuário aceitou os termos específicos e não revogou.
    /// </summary>
    Task<bool> HasAcceptedAsync(Guid userId, Guid termsId, CancellationToken cancellationToken);

    /// <summary>
    /// Adiciona um novo aceite.
    /// </summary>
    Task AddAsync(TermsAcceptance acceptance, CancellationToken cancellationToken);

    /// <summary>
    /// Atualiza um aceite existente.
    /// </summary>
    Task UpdateAsync(TermsAcceptance acceptance, CancellationToken cancellationToken);
}
