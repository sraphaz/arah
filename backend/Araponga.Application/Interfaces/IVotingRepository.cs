using Araponga.Domain.Governance;

namespace Araponga.Application.Interfaces;

/// <summary>
/// Repositório para gerenciar votações comunitárias.
/// </summary>
public interface IVotingRepository
{
    /// <summary>
    /// Adiciona uma nova votação.
    /// </summary>
    Task AddAsync(Voting voting, CancellationToken cancellationToken);

    /// <summary>
    /// Atualiza uma votação existente.
    /// </summary>
    Task UpdateAsync(Voting voting, CancellationToken cancellationToken);

    /// <summary>
    /// Busca uma votação pelo ID.
    /// </summary>
    Task<Voting?> GetByIdAsync(Guid votingId, CancellationToken cancellationToken);

    /// <summary>
    /// Lista votações de um território, opcionalmente filtradas por status e criador.
    /// </summary>
    Task<IReadOnlyList<Voting>> ListByTerritoryAsync(
        Guid territoryId,
        VotingStatus? status,
        Guid? createdByUserId,
        CancellationToken cancellationToken);
}
