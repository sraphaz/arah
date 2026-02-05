namespace Araponga.Infrastructure.Postgres.Entities;

/// <summary>
/// Registro Postgres para preferências de mídia do usuário.
/// </summary>
public sealed class UserMediaPreferencesRecord
{
    public Guid UserId { get; set; }
    public bool ShowImages { get; set; } = true;
    public bool ShowVideos { get; set; } = true;
    public bool ShowAudio { get; set; } = true;
    public bool AutoPlayVideos { get; set; }
    public bool AutoPlayAudio { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
