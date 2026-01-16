namespace Araponga.Application.Interfaces.Media;

/// <summary>
/// Serviço responsável pelo processamento de mídias (redimensionamento, otimização, validação).
/// </summary>
public interface IMediaProcessingService
{
    /// <summary>
    /// Redimensiona uma imagem mantendo o aspect ratio.
    /// </summary>
    /// <param name="stream">Stream da imagem original.</param>
    /// <param name="maxWidth">Largura máxima em pixels.</param>
    /// <param name="maxHeight">Altura máxima em pixels.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Stream da imagem redimensionada.</returns>
    Task<Stream> ResizeImageAsync(Stream stream, int maxWidth, int maxHeight, CancellationToken cancellationToken = default);

    /// <summary>
    /// Otimiza uma imagem (compressão, redução de qualidade, etc.).
    /// </summary>
    /// <param name="stream">Stream da imagem original.</param>
    /// <param name="mimeType">Tipo MIME da imagem (ex: "image/jpeg").</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Stream da imagem otimizada.</returns>
    Task<Stream> OptimizeImageAsync(Stream stream, string mimeType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Valida se um stream contém uma imagem válida e retorna suas dimensões.
    /// </summary>
    /// <param name="stream">Stream da imagem.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Tupla contendo largura e altura em pixels, ou null se não for uma imagem válida.</returns>
    Task<(int Width, int Height)?> GetImageDimensionsAsync(Stream stream, CancellationToken cancellationToken = default);

    /// <summary>
    /// Valida se um stream contém uma imagem válida do tipo especificado.
    /// </summary>
    /// <param name="stream">Stream da imagem.</param>
    /// <param name="mimeType">Tipo MIME esperado.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>True se a imagem é válida, false caso contrário.</returns>
    Task<bool> ValidateImageAsync(Stream stream, string mimeType, CancellationToken cancellationToken = default);
}