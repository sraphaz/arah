namespace Araponga.Modules.Chat.Infrastructure.Postgres.Entities;

public sealed class ChatConversationStatsRecord
{
    public Guid ConversationId { get; set; }
    public Guid? LastMessageId { get; set; }
    public DateTime? LastMessageAtUtc { get; set; }
    public Guid? LastSenderUserId { get; set; }
    public string? LastPreview { get; set; }
    public long MessageCount { get; set; }
}
