namespace Araponga.Infrastructure.Shared.Postgres.Entities;

public sealed class UserDeviceRecord
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string DeviceToken { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty; // "ios", "android", "web"
    public string? DeviceName { get; set; }
    public DateTime RegisteredAtUtc { get; set; }
    public DateTime? LastUsedAtUtc { get; set; }
    public bool IsActive { get; set; }
}
