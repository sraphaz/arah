namespace Araponga.Infrastructure.Media;

/// <summary>
/// Opções de configuração para o sistema de armazenamento de mídia.
/// </summary>
public sealed class MediaStorageOptions
{
    /// <summary>
    /// Provider de armazenamento (Local, S3, AzureBlob).
    /// </summary>
    public string Provider { get; set; } = "Local";

    /// <summary>
    /// Caminho base para armazenamento local (ex: "wwwroot/media").
    /// </summary>
    public string LocalPath { get; set; } = "wwwroot/media";

    /// <summary>
    /// Tamanho máximo para imagens em bytes (padrão: 10MB).
    /// </summary>
    public long MaxImageSizeBytes { get; set; } = 10 * 1024 * 1024; // 10MB

    /// <summary>
    /// Tamanho máximo para vídeos em bytes (padrão: 50MB).
    /// </summary>
    public long MaxVideoSizeBytes { get; set; } = 50 * 1024 * 1024; // 50MB

    /// <summary>
    /// Tamanho máximo para áudios em bytes (padrão: 20MB).
    /// </summary>
    public long MaxAudioSizeBytes { get; set; } = 20 * 1024 * 1024; // 20MB

    /// <summary>
    /// Largura máxima para imagens em pixels (padrão: 4000px).
    /// </summary>
    public int MaxImageWidthPx { get; set; } = 4000;

    /// <summary>
    /// Altura máxima para imagens em pixels (padrão: 4000px).
    /// </summary>
    public int MaxImageHeightPx { get; set; } = 4000;

    /// <summary>
    /// Largura máxima após redimensionamento automático (padrão: 1920px).
    /// </summary>
    public int AutoResizeMaxWidthPx { get; set; } = 1920;

    /// <summary>
    /// Altura máxima após redimensionamento automático (padrão: 1920px).
    /// </summary>
    public int AutoResizeMaxHeightPx { get; set; } = 1920;

    /// <summary>
    /// Tipos MIME permitidos para imagens.
    /// </summary>
    public IReadOnlySet<string> AllowedImageMimeTypes { get; set; } = new HashSet<string>
    {
        "image/jpeg",
        "image/png",
        "image/webp"
    };

    /// <summary>
    /// Tipos MIME permitidos para vídeos.
    /// </summary>
    public IReadOnlySet<string> AllowedVideoMimeTypes { get; set; } = new HashSet<string>
    {
        "video/mp4"
    };

    /// <summary>
    /// Tipos MIME permitidos para áudios.
    /// </summary>
    public IReadOnlySet<string> AllowedAudioMimeTypes { get; set; } = new HashSet<string>
    {
        "audio/mpeg",      // MP3
        "audio/mp3",       // MP3 (alternativo)
        "audio/wav",       // WAV
        "audio/x-wav",     // WAV (alternativo)
        "audio/ogg",       // OGG
        "audio/vorbis"     // OGG Vorbis
    };

    // S3 Configuration
    public string? S3BucketName { get; set; }
    public string? S3Region { get; set; }
    public string? S3AccessKeyId { get; set; }
    public string? S3SecretAccessKey { get; set; }
    public string? S3Prefix { get; set; } // Prefixo opcional para organização
    public string? S3ServiceUrl { get; set; } // Endpoint customizado (ex: MinIO http://localhost:9000)
    public bool S3ForcePathStyle { get; set; } = false; // Para MinIO e outros S3-compatible

    // Azure Blob Configuration
    public string? AzureBlobConnectionString { get; set; }
    public string? AzureBlobContainerName { get; set; }
    public string? AzureBlobPrefix { get; set; } // Prefixo opcional para organização

    // Cache Configuration
    public bool EnableUrlCache { get; set; } = true;
    public TimeSpan? UrlCacheExpiration { get; set; } = TimeSpan.FromHours(24);

    // Async Processing Configuration
    public bool EnableAsyncProcessing { get; set; } = true;
    public long AsyncProcessingThresholdBytes { get; set; } = 5 * 1024 * 1024; // 5MB
}