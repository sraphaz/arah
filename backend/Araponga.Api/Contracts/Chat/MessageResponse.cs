namespace Araponga.Api.Contracts.Chat;

public sealed record MessageResponse(
    Guid Id,
    Guid ConversationId,
    Guid SenderUserId,
    string ContentType,
    string? Text,
    DateTime CreatedAtUtc,
    DateTime? EditedAtUtc);

