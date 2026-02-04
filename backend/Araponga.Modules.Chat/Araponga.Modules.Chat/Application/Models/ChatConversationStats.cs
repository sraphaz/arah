namespace Araponga.Application.Models;

public sealed record ChatConversationStats(
    Guid ConversationId,
    Guid? LastMessageId,
    DateTime? LastMessageAtUtc,
    Guid? LastSenderUserId,
    string? LastPreview,
    long MessageCount);
