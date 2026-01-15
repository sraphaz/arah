using Araponga.Domain.Chat;

namespace Araponga.Infrastructure.Postgres.Entities;

public sealed class ChatMessageRecord
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public Guid SenderUserId { get; set; }
    public MessageContentType ContentType { get; set; }
    public string? Text { get; set; }
    public string? PayloadJson { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? EditedAtUtc { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public Guid? DeletedByUserId { get; set; }
}

