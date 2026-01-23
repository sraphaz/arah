using Araponga.Domain.Governance;

namespace Araponga.Application.Interfaces;

/// <summary>
/// Repositório para gerenciar votos individuais.
/// </summary>
public interface IVoteRepository
{
    /// <summary>
    /// Adiciona um novo voto.
    /// </summary>
    Task AddAsync(Vote vote, CancellationToken cancellationToken);

    /// <summary>
    /// Verifica se um usuário já votou em uma votação.
    /// </summary>
    Task<bool> HasVotedAsync(Guid votingId, Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Lista todos os votos de uma votação.
    /// </summary>
    Task<IReadOnlyList<Vote>> ListByVotingIdAsync(Guid votingId, CancellationToken cancellationToken);

    /// <summary>
    /// Conta votos por opção em uma votação.
    /// </summary>
    Task<Dictionary<string, int>> CountByOptionAsync(Guid votingId, CancellationToken cancellationToken);

    /// <summary>
    /// Lista todos os votos de um usuário.
    /// </summary>
    Task<IReadOnlyList<Vote>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken);
}
