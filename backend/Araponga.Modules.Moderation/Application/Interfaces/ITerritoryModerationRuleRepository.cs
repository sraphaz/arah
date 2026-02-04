using Araponga.Domain.Governance;

namespace Araponga.Modules.Moderation.Application.Interfaces;

/// <summary>
/// Repositório para gerenciar regras de moderação comunitária.
/// </summary>
public interface ITerritoryModerationRuleRepository
{
    Task AddAsync(TerritoryModerationRule rule, CancellationToken cancellationToken);
    Task UpdateAsync(TerritoryModerationRule rule, CancellationToken cancellationToken);
    Task<IReadOnlyList<TerritoryModerationRule>> ListByTerritoryAsync(Guid territoryId, bool? isActive, CancellationToken cancellationToken);
    Task<TerritoryModerationRule?> GetByIdAsync(Guid ruleId, CancellationToken cancellationToken);
}
