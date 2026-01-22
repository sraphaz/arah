namespace Araponga.Api.Contracts.Devices;

public sealed record DeviceResponse(
    Guid Id,
    string Platform,
    string? DeviceName,
    DateTime RegisteredAtUtc,
    DateTime? LastUsedAtUtc,
    bool IsActive
);
