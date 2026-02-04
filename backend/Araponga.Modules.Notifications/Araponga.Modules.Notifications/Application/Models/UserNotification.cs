namespace Araponga.Application.Models;

public sealed record UserNotification(
    Guid Id,
    Guid UserId,
    string Title,
    string? Body,
    string Kind,
    string? DataJson,
    DateTime CreatedAtUtc,
    DateTime? ReadAtUtc,
    Guid? SourceOutboxId
);
