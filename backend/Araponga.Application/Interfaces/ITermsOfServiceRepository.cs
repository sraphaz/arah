using Araponga.Domain.Policies;

namespace Araponga.Application.Interfaces;

/// <summary>
/// Repositório para gerenciar Termos de Uso.
/// </summary>
public interface ITermsOfServiceRepository
{
    /// <summary>
    /// Obtém termos de serviço por ID.
    /// </summary>
    Task<TermsOfService?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Obtém termos de serviço por versão.
    /// </summary>
    Task<TermsOfService?> GetByVersionAsync(string version, CancellationToken cancellationToken);

    /// <summary>
    /// Obtém todos os termos de serviço ativos.
    /// </summary>
    Task<IReadOnlyList<TermsOfService>> GetActiveAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Lista todos os termos de serviço (ativos e inativos).
    /// </summary>
    Task<IReadOnlyList<TermsOfService>> ListAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Adiciona novos termos de serviço.
    /// </summary>
    Task AddAsync(TermsOfService terms, CancellationToken cancellationToken);

    /// <summary>
    /// Atualiza termos de serviço existentes.
    /// </summary>
    Task UpdateAsync(TermsOfService terms, CancellationToken cancellationToken);
}
