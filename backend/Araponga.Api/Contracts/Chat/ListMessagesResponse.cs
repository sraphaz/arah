namespace Araponga.Api.Contracts.Chat;

public sealed record ListMessagesResponse(
    Guid ConversationId,
    IReadOnlyList<MessageResponse> Messages,
    DateTime? NextCursorCreatedAtUtc,
    Guid? NextCursorMessageId);

