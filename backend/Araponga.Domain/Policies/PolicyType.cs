namespace Araponga.Domain.Policies;

/// <summary>
/// Tipos de políticas e termos que podem ser aceitos pelos usuários.
/// </summary>
public enum PolicyType
{
    /// <summary>
    /// Termos de Uso - condições gerais de uso da plataforma.
    /// </summary>
    TermsOfService = 1,

    /// <summary>
    /// Política de Privacidade - como os dados são coletados e utilizados.
    /// </summary>
    PrivacyPolicy = 2,

    /// <summary>
    /// Diretrizes Comunitárias - regras de comportamento na comunidade.
    /// </summary>
    CommunityGuidelines = 3,

    /// <summary>
    /// Política do Marketplace - regras para venda e compra.
    /// </summary>
    MarketplacePolicy = 4,

    /// <summary>
    /// Política de Eventos - regras para criação e participação em eventos.
    /// </summary>
    EventPolicy = 5,

    /// <summary>
    /// Política de Moderação - regras para moderadores.
    /// </summary>
    ModerationPolicy = 6
}
