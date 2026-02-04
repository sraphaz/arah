using Araponga.Domain.Territories;

namespace Araponga.Application.Interfaces;

/// <summary>
/// Repositório para gerenciar caracterização de territórios.
/// </summary>
public interface ITerritoryCharacterizationRepository
{
    /// <summary>
    /// Adiciona ou atualiza a caracterização de um território.
    /// </summary>
    Task UpsertAsync(TerritoryCharacterization characterization, CancellationToken cancellationToken);

    /// <summary>
    /// Busca a caracterização de um território.
    /// </summary>
    Task<TerritoryCharacterization?> GetByTerritoryIdAsync(Guid territoryId, CancellationToken cancellationToken);
}
