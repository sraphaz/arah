namespace Araponga.Api.Contracts.Notifications;

public sealed record NotificationResponse(
    Guid Id,
    string Title,
    string? Body,
    string Kind,
    string? DataJson,
    DateTime CreatedAtUtc,
    DateTime? ReadAtUtc
);
