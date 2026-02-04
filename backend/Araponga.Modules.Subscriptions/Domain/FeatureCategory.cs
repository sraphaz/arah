namespace Araponga.Domain.Subscriptions;

/// <summary>
/// Categoria de funcionalidades.
/// </summary>
public enum FeatureCategory
{
    /// <summary>
    /// Funcionalidades core - sempre no FREE.
    /// </summary>
    Core = 0,

    /// <summary>
    /// Funcionalidades melhoradas.
    /// </summary>
    Enhanced = 1,

    /// <summary>
    /// Funcionalidades premium.
    /// </summary>
    Premium = 2,

    /// <summary>
    /// Funcionalidades empresariais.
    /// </summary>
    Enterprise = 3
}
