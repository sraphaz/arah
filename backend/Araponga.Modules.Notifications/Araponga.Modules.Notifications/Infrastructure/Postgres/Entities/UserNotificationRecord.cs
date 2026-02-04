namespace Araponga.Modules.Notifications.Infrastructure.Postgres.Entities;

public sealed class UserNotificationRecord
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Body { get; set; }
    public string Kind { get; set; } = string.Empty;
    public string? DataJson { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? ReadAtUtc { get; set; }
    public Guid? SourceOutboxId { get; set; }
}
