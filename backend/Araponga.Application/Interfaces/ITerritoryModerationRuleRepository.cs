using Araponga.Domain.Governance;

namespace Araponga.Application.Interfaces;

/// <summary>
/// Repositório para gerenciar regras de moderação comunitária.
/// </summary>
public interface ITerritoryModerationRuleRepository
{
    /// <summary>
    /// Adiciona uma nova regra.
    /// </summary>
    Task AddAsync(TerritoryModerationRule rule, CancellationToken cancellationToken);

    /// <summary>
    /// Atualiza uma regra existente.
    /// </summary>
    Task UpdateAsync(TerritoryModerationRule rule, CancellationToken cancellationToken);

    /// <summary>
    /// Lista regras de um território, opcionalmente filtradas por status ativo.
    /// </summary>
    Task<IReadOnlyList<TerritoryModerationRule>> ListByTerritoryAsync(
        Guid territoryId,
        bool? isActive,
        CancellationToken cancellationToken);

    /// <summary>
    /// Busca uma regra pelo ID.
    /// </summary>
    Task<TerritoryModerationRule?> GetByIdAsync(Guid ruleId, CancellationToken cancellationToken);
}
