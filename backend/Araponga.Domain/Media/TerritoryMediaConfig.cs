namespace Araponga.Domain.Media;

/// <summary>
/// Configuração de mídias para um território, permitindo controle granular por tipo de conteúdo.
/// </summary>
public sealed class TerritoryMediaConfig
{
    public Guid TerritoryId { get; set; }

    /// <summary>
    /// Configuração de mídias para Posts.
    /// </summary>
    public MediaContentConfig Posts { get; set; } = new();

    /// <summary>
    /// Configuração de mídias para Eventos.
    /// </summary>
    public MediaContentConfig Events { get; set; } = new();

    /// <summary>
    /// Configuração de mídias para Marketplace (Items).
    /// </summary>
    public MediaContentConfig Marketplace { get; set; } = new();

    /// <summary>
    /// Configuração de mídias para Chat.
    /// </summary>
    public MediaChatConfig Chat { get; set; } = new();

    /// <summary>
    /// Data/hora UTC da última atualização.
    /// </summary>
    public DateTime UpdatedAtUtc { get; set; }

    /// <summary>
    /// ID do usuário que atualizou a configuração (Curator).
    /// </summary>
    public Guid? UpdatedByUserId { get; set; }
}

/// <summary>
/// Configuração de mídias para um tipo de conteúdo (Posts, Eventos, Marketplace).
/// </summary>
public sealed class MediaContentConfig
{
    /// <summary>
    /// Indica se imagens estão habilitadas.
    /// </summary>
    public bool ImagesEnabled { get; set; } = true;

    /// <summary>
    /// Indica se vídeos estão habilitados.
    /// </summary>
    public bool VideosEnabled { get; set; } = true;

    /// <summary>
    /// Indica se áudios estão habilitados.
    /// </summary>
    public bool AudioEnabled { get; set; } = true;

    /// <summary>
    /// Quantidade máxima de mídias no total (imagens + vídeos + áudios).
    /// </summary>
    public int MaxMediaCount { get; set; } = 10;

    /// <summary>
    /// Quantidade máxima de vídeos permitidos.
    /// </summary>
    public int MaxVideoCount { get; set; } = 1;

    /// <summary>
    /// Quantidade máxima de áudios permitidos.
    /// </summary>
    public int MaxAudioCount { get; set; } = 1;

    /// <summary>
    /// Tamanho máximo de imagens em bytes (padrão: 10MB).
    /// </summary>
    public long MaxImageSizeBytes { get; set; } = 10 * 1024 * 1024; // 10MB

    /// <summary>
    /// Tamanho máximo de vídeos em bytes (padrão: 50MB para posts, 100MB para eventos, 30MB para items).
    /// </summary>
    public long MaxVideoSizeBytes { get; set; } = 50 * 1024 * 1024; // 50MB

    /// <summary>
    /// Tamanho máximo de áudios em bytes (padrão: 10MB para posts, 20MB para eventos, 5MB para items).
    /// </summary>
    public long MaxAudioSizeBytes { get; set; } = 10 * 1024 * 1024; // 10MB

    /// <summary>
    /// Duração máxima de vídeos em segundos (validação futura).
    /// </summary>
    public int? MaxVideoDurationSeconds { get; set; } = null;

    /// <summary>
    /// Duração máxima de áudios em segundos (validação futura).
    /// </summary>
    public int? MaxAudioDurationSeconds { get; set; } = null;

    /// <summary>
    /// Tipos MIME permitidos para imagens (null ou vazio = usar global de MediaStorageOptions).
    /// </summary>
    public List<string>? AllowedImageMimeTypes { get; set; } = null;

    /// <summary>
    /// Tipos MIME permitidos para vídeos (null ou vazio = usar global de MediaStorageOptions).
    /// </summary>
    public List<string>? AllowedVideoMimeTypes { get; set; } = null;

    /// <summary>
    /// Tipos MIME permitidos para áudios (null ou vazio = usar global de MediaStorageOptions).
    /// </summary>
    public List<string>? AllowedAudioMimeTypes { get; set; } = null;
}

/// <summary>
/// Configuração de mídias para Chat.
/// </summary>
public sealed class MediaChatConfig
{
    /// <summary>
    /// Indica se imagens estão habilitadas no chat.
    /// </summary>
    public bool ImagesEnabled { get; set; } = true;

    /// <summary>
    /// Indica se áudios (mensagens de voz) estão habilitados no chat.
    /// </summary>
    public bool AudioEnabled { get; set; } = true;

    /// <summary>
    /// Vídeos nunca são permitidos no chat (sempre false).
    /// </summary>
    public bool VideosEnabled { get; set; } = false;

    /// <summary>
    /// Tamanho máximo de imagens em bytes (padrão: 5MB).
    /// </summary>
    public long MaxImageSizeBytes { get; set; } = 5 * 1024 * 1024; // 5MB

    /// <summary>
    /// Tamanho máximo de áudios em bytes (padrão: 2MB para mensagens de voz).
    /// </summary>
    public long MaxAudioSizeBytes { get; set; } = 2 * 1024 * 1024; // 2MB

    /// <summary>
    /// Duração máxima de áudios em segundos (padrão: 60s para mensagens de voz).
    /// </summary>
    public int? MaxAudioDurationSeconds { get; set; } = 60;

    /// <summary>
    /// Tipos MIME permitidos para imagens no chat (null ou vazio = usar global de MediaStorageOptions).
    /// </summary>
    public List<string>? AllowedImageMimeTypes { get; set; } = null;

    /// <summary>
    /// Tipos MIME permitidos para áudios no chat (null ou vazio = usar global de MediaStorageOptions).
    /// </summary>
    public List<string>? AllowedAudioMimeTypes { get; set; } = null;
}
