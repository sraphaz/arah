namespace Araponga.Application.Interfaces.Media;

/// <summary>
/// Interface para acessar limites globais de mídia (valores padrão do sistema).
/// </summary>
public interface IGlobalMediaLimits
{
    /// <summary>
    /// Tamanho máximo para imagens em bytes.
    /// </summary>
    long MaxImageSizeBytes { get; }

    /// <summary>
    /// Tamanho máximo para vídeos em bytes.
    /// </summary>
    long MaxVideoSizeBytes { get; }

    /// <summary>
    /// Tamanho máximo para áudios em bytes.
    /// </summary>
    long MaxAudioSizeBytes { get; }

    /// <summary>
    /// Tipos MIME permitidos para imagens.
    /// </summary>
    IReadOnlySet<string> AllowedImageMimeTypes { get; }

    /// <summary>
    /// Tipos MIME permitidos para vídeos.
    /// </summary>
    IReadOnlySet<string> AllowedVideoMimeTypes { get; }

    /// <summary>
    /// Tipos MIME permitidos para áudios.
    /// </summary>
    IReadOnlySet<string> AllowedAudioMimeTypes { get; }
}
