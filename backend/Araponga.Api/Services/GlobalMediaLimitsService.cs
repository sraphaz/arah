using Araponga.Application.Interfaces.Media;
using Araponga.Infrastructure.Media;
using Microsoft.Extensions.Options;

namespace Araponga.Api.Services;

/// <summary>
/// Implementação de IGlobalMediaLimits usando MediaStorageOptions da Infrastructure.
/// Este serviço é registrado na camada Api para injetar MediaStorageOptions.
/// </summary>
public sealed class GlobalMediaLimitsService : IGlobalMediaLimits
{
    private readonly MediaStorageOptions _options;

    public GlobalMediaLimitsService(IOptions<MediaStorageOptions> options)
    {
        _options = options.Value;
    }

    public long MaxImageSizeBytes => _options.MaxImageSizeBytes;
    public long MaxVideoSizeBytes => _options.MaxVideoSizeBytes;
    public long MaxAudioSizeBytes => _options.MaxAudioSizeBytes;
    public IReadOnlySet<string> AllowedImageMimeTypes => _options.AllowedImageMimeTypes;
    public IReadOnlySet<string> AllowedVideoMimeTypes => _options.AllowedVideoMimeTypes;
    public IReadOnlySet<string> AllowedAudioMimeTypes => _options.AllowedAudioMimeTypes;
}
