namespace Araponga.Domain.Governance;

/// <summary>
/// Tipos de regras de moderação comunitária.
/// </summary>
public enum RuleType
{
    /// <summary>
    /// Tipos de conteúdo permitidos.
    /// </summary>
    ContentType = 0,

    /// <summary>
    /// Palavras/temas proibidos.
    /// </summary>
    ProhibitedWords = 1,

    /// <summary>
    /// Regras de comportamento.
    /// </summary>
    Behavior = 2,

    /// <summary>
    /// Política de marketplace.
    /// </summary>
    MarketplacePolicy = 3,

    /// <summary>
    /// Política de eventos.
    /// </summary>
    EventPolicy = 4
}
