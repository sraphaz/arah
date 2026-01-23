namespace Araponga.Domain.Governance;

/// <summary>
/// Tipos de votações comunitárias.
/// </summary>
public enum VotingType
{
    /// <summary>
    /// Priorização de temas (quais aparecem mais no feed).
    /// </summary>
    ThemePrioritization = 0,

    /// <summary>
    /// Regra de moderação (o que é permitido/não permitido).
    /// </summary>
    ModerationRule = 1,

    /// <summary>
    /// Caracterização do território (tags que descrevem).
    /// </summary>
    TerritoryCharacterization = 2,

    /// <summary>
    /// Feature flag territorial (quais funcionalidades estão ativas).
    /// </summary>
    FeatureFlag = 3,

    /// <summary>
    /// Política comunitária (regras de convivência).
    /// </summary>
    CommunityPolicy = 4
}
