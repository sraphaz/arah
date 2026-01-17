using Araponga.Application.Interfaces.Media;

namespace Araponga.Infrastructure.InMemory;

/// <summary>
/// Implementação in-memory de IGlobalMediaLimits para testes.
/// </summary>
public sealed class InMemoryGlobalMediaLimits : IGlobalMediaLimits
{
    public long MaxImageSizeBytes => 10 * 1024 * 1024; // 10MB
    public long MaxVideoSizeBytes => 100 * 1024 * 1024; // 100MB
    public long MaxAudioSizeBytes => 20 * 1024 * 1024; // 20MB

    public IReadOnlySet<string> AllowedImageMimeTypes => new HashSet<string>
    {
        "image/jpeg",
        "image/png",
        "image/gif",
        "image/webp"
    };

    public IReadOnlySet<string> AllowedVideoMimeTypes => new HashSet<string>
    {
        "video/mp4",
        "video/webm",
        "video/ogg"
    };

    public IReadOnlySet<string> AllowedAudioMimeTypes => new HashSet<string>
    {
        "audio/mpeg",
        "audio/ogg",
        "audio/wav"
    };
}
