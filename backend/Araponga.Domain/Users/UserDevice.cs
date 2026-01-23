namespace Araponga.Domain.Users;

/// <summary>
/// Representa um dispositivo registrado para receber notificações push.
/// </summary>
public sealed class UserDevice
{
    public Guid Id { get; }
    public Guid UserId { get; }
    public string DeviceToken { get; private set; }
    public string Platform { get; } // "ios", "android", "web"
    public string? DeviceName { get; private set; }
    public DateTime RegisteredAtUtc { get; }
    public DateTime? LastUsedAtUtc { get; private set; }
    public bool IsActive { get; private set; }

    public UserDevice(
        Guid id,
        Guid userId,
        string deviceToken,
        string platform,
        string? deviceName,
        DateTime registeredAtUtc)
    {
        if (string.IsNullOrWhiteSpace(deviceToken))
        {
            throw new ArgumentException("Device token is required.", nameof(deviceToken));
        }

        if (string.IsNullOrWhiteSpace(platform))
        {
            throw new ArgumentException("Platform is required.", nameof(platform));
        }

        Id = id;
        UserId = userId;
        DeviceToken = deviceToken;
        Platform = platform;
        DeviceName = deviceName;
        RegisteredAtUtc = registeredAtUtc;
        LastUsedAtUtc = registeredAtUtc;
        IsActive = true;
    }

    public void UpdateToken(string newToken)
    {
        if (string.IsNullOrWhiteSpace(newToken))
        {
            throw new ArgumentException("Device token is required.", nameof(newToken));
        }

        DeviceToken = newToken;
        LastUsedAtUtc = DateTime.UtcNow;
    }

    public void UpdateDeviceName(string? deviceName)
    {
        DeviceName = deviceName;
        LastUsedAtUtc = DateTime.UtcNow;
    }

    public void MarkAsActive()
    {
        IsActive = true;
        LastUsedAtUtc = DateTime.UtcNow;
    }

    public void MarkAsInactive()
    {
        IsActive = false;
    }
}
