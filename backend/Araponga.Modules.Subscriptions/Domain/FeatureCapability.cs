namespace Araponga.Domain.Subscriptions;

/// <summary>
/// Capacidades de funcionalidades que podem ser liberadas por plano.
/// </summary>
public enum FeatureCapability
{
    /// <summary>
    /// Feed básico - sempre no FREE.
    /// </summary>
    FeedBasic = 0,

    /// <summary>
    /// Posts básicos - sempre no FREE.
    /// </summary>
    PostsBasic = 1,

    /// <summary>
    /// Posts ilimitados.
    /// </summary>
    PostsUnlimited = 2,

    /// <summary>
    /// Eventos básicos - sempre no FREE.
    /// </summary>
    EventsBasic = 3,

    /// <summary>
    /// Eventos ilimitados.
    /// </summary>
    EventsUnlimited = 4,

    /// <summary>
    /// Marketplace básico - sempre no FREE.
    /// </summary>
    MarketplaceBasic = 5,

    /// <summary>
    /// Marketplace avançado.
    /// </summary>
    MarketplaceAdvanced = 6,

    /// <summary>
    /// Chat básico - sempre no FREE.
    /// </summary>
    ChatBasic = 7,

    /// <summary>
    /// Analytics e métricas.
    /// </summary>
    Analytics = 8,

    /// <summary>
    /// Integração com IA.
    /// </summary>
    AIIntegration = 9,

    /// <summary>
    /// Suporte prioritário.
    /// </summary>
    PrioritySupport = 10,

    /// <summary>
    /// Branding customizado.
    /// </summary>
    CustomBranding = 11,

    /// <summary>
    /// Acesso à API.
    /// </summary>
    APIAccess = 12,

    /// <summary>
    /// Governança avançada.
    /// </summary>
    AdvancedGovernance = 13,

    /// <summary>
    /// Recursos premium territoriais.
    /// </summary>
    TerritoryPremium = 14
}
