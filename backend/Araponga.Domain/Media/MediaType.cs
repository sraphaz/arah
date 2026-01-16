namespace Araponga.Domain.Media;

/// <summary>
/// Tipos de mídia suportados.
/// </summary>
public enum MediaType
{
    /// <summary>
    /// Imagem (JPEG, PNG, WebP)
    /// </summary>
    Image = 1,

    /// <summary>
    /// Vídeo (MP4)
    /// </summary>
    Video = 2,

    /// <summary>
    /// Áudio (MP3, WAV)
    /// </summary>
    Audio = 3,

    /// <summary>
    /// Documento (PDF, DOCX, etc.)
    /// </summary>
    Document = 4
}