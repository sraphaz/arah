namespace Araponga.Application.Interfaces.Media;

/// <summary>
/// Serviço responsável pela validação de mídias (tipo, tamanho, formato).
/// </summary>
public interface IMediaValidator
{
    /// <summary>
    /// Valida uma mídia (tipo MIME, tamanho, formato).
    /// </summary>
    /// <param name="stream">Stream da mídia.</param>
    /// <param name="mimeType">Tipo MIME da mídia.</param>
    /// <param name="sizeBytes">Tamanho do arquivo em bytes.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Resultado da validação contendo erros, se houver.</returns>
    Task<MediaValidationResult> ValidateAsync(Stream stream, string mimeType, long sizeBytes, CancellationToken cancellationToken = default);
}

/// <summary>
/// Resultado da validação de mídia.
/// </summary>
public sealed class MediaValidationResult
{
    public MediaValidationResult(bool isValid, IReadOnlyList<string> errors)
    {
        IsValid = isValid;
        Errors = errors ?? Array.Empty<string>();
    }

    public bool IsValid { get; }
    public IReadOnlyList<string> Errors { get; }

    public static MediaValidationResult Success() => new(true, Array.Empty<string>());
    public static MediaValidationResult Failure(params string[] errors) => new(false, errors);
}