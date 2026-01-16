using Araponga.Application.Interfaces.Media;
using Araponga.Infrastructure.Media;
using Microsoft.Extensions.Options;

namespace Araponga.Infrastructure.Media;

/// <summary>
/// Validador de mídias implementando validações de tipo MIME, tamanho e formato.
/// </summary>
public sealed class MediaValidator : IMediaValidator
{
    private readonly MediaStorageOptions _options;

    public MediaValidator(IOptions<MediaStorageOptions> options)
    {
        _options = options.Value;
    }

    public Task<MediaValidationResult> ValidateAsync(Stream stream, string mimeType, long sizeBytes, CancellationToken cancellationToken = default)
    {
        var errors = new List<string>();

        // Validação de tipo MIME
        if (!IsAllowedMimeType(mimeType))
        {
            errors.Add($"Tipo MIME '{mimeType}' não é permitido.");
            return Task.FromResult(MediaValidationResult.Failure(errors.ToArray()));
        }

        // Validação de tamanho
        var maxSize = GetMaxSizeForMimeType(mimeType);
        if (sizeBytes > maxSize)
        {
            var maxSizeMB = maxSize / (1024.0 * 1024.0);
            errors.Add($"Arquivo excede o tamanho máximo permitido de {maxSizeMB:F1}MB.");
            return Task.FromResult(MediaValidationResult.Failure(errors.ToArray()));
        }

        // Validação básica de tamanho de stream (se possível)
        if (stream.CanSeek && stream.Length != sizeBytes)
        {
            errors.Add("Tamanho do arquivo não corresponde ao tamanho informado.");
            return Task.FromResult(MediaValidationResult.Failure(errors.ToArray()));
        }

        return Task.FromResult(MediaValidationResult.Success());
    }

    private bool IsAllowedMimeType(string mimeType)
    {
        return _options.AllowedImageMimeTypes.Contains(mimeType, StringComparer.OrdinalIgnoreCase) ||
               _options.AllowedVideoMimeTypes.Contains(mimeType, StringComparer.OrdinalIgnoreCase);
    }

    private long GetMaxSizeForMimeType(string mimeType)
    {
        if (_options.AllowedImageMimeTypes.Contains(mimeType, StringComparer.OrdinalIgnoreCase))
        {
            return _options.MaxImageSizeBytes;
        }

        if (_options.AllowedVideoMimeTypes.Contains(mimeType, StringComparer.OrdinalIgnoreCase))
        {
            return _options.MaxVideoSizeBytes;
        }

        // Tipo não reconhecido - retorna limite conservador
        return _options.MaxImageSizeBytes;
    }
}