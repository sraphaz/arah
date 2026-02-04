using Araponga.Domain.Chat;

namespace Araponga.Modules.Chat.Infrastructure.Postgres.Entities;

public sealed class ChatConversationParticipantRecord
{
    public Guid ConversationId { get; set; }
    public Guid UserId { get; set; }
    public ConversationParticipantRole Role { get; set; }
    public DateTime JoinedAtUtc { get; set; }
    public DateTime? LeftAtUtc { get; set; }
    public DateTime? MutedUntilUtc { get; set; }
    public Guid? LastReadMessageId { get; set; }
    public DateTime? LastReadAtUtc { get; set; }
}
