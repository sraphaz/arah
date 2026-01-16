namespace Araponga.Domain.Media;

/// <summary>
/// Tipos de entidades que podem possuir mídias.
/// </summary>
public enum MediaOwnerType
{
    /// <summary>
    /// Usuário (avatar/foto de perfil)
    /// </summary>
    User = 1,

    /// <summary>
    /// Post do feed
    /// </summary>
    Post = 2,

    /// <summary>
    /// Evento territorial
    /// </summary>
    Event = 3,

    /// <summary>
    /// Item do marketplace (anúncio)
    /// </summary>
    StoreItem = 4,

    /// <summary>
    /// Mensagem de chat
    /// </summary>
    ChatMessage = 5
}