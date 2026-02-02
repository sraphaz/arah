using Araponga.Domain.Chat;
using Araponga.Modules.Chat.Infrastructure.Postgres.Entities;

namespace Araponga.Modules.Chat.Infrastructure.Postgres;

public static class ChatMappers
{
    public static ChatConversationRecord ToRecord(this ChatConversation conversation)
    {
        return new ChatConversationRecord
        {
            Id = conversation.Id,
            TerritoryId = conversation.TerritoryId,
            Kind = conversation.Kind,
            Status = conversation.Status,
            Name = conversation.Name,
            CreatedByUserId = conversation.CreatedByUserId,
            CreatedAtUtc = conversation.CreatedAtUtc,
            ApprovedByUserId = conversation.ApprovedByUserId,
            ApprovedAtUtc = conversation.ApprovedAtUtc,
            LockedByUserId = conversation.LockedByUserId,
            LockedAtUtc = conversation.LockedAtUtc,
            DisabledByUserId = conversation.DisabledByUserId,
            DisabledAtUtc = conversation.DisabledAtUtc
        };
    }

    public static ChatConversation ToDomain(this ChatConversationRecord record)
    {
        return new ChatConversation(
            record.Id,
            record.Kind,
            record.Status,
            record.TerritoryId,
            record.Name,
            record.CreatedByUserId,
            record.CreatedAtUtc,
            record.ApprovedByUserId,
            record.ApprovedAtUtc,
            record.LockedAtUtc,
            record.LockedByUserId,
            record.DisabledAtUtc,
            record.DisabledByUserId);
    }

    public static ChatConversationParticipantRecord ToRecord(this ConversationParticipant participant)
    {
        return new ChatConversationParticipantRecord
        {
            ConversationId = participant.ConversationId,
            UserId = participant.UserId,
            Role = participant.Role,
            JoinedAtUtc = participant.JoinedAtUtc,
            LeftAtUtc = participant.LeftAtUtc,
            MutedUntilUtc = participant.MutedUntilUtc,
            LastReadMessageId = participant.LastReadMessageId,
            LastReadAtUtc = participant.LastReadAtUtc
        };
    }

    public static ConversationParticipant ToDomain(this ChatConversationParticipantRecord record)
    {
        return new ConversationParticipant(
            record.ConversationId,
            record.UserId,
            record.Role,
            record.JoinedAtUtc,
            record.LeftAtUtc,
            record.MutedUntilUtc,
            record.LastReadMessageId,
            record.LastReadAtUtc);
    }

    public static ChatMessageRecord ToRecord(this ChatMessage message)
    {
        return new ChatMessageRecord
        {
            Id = message.Id,
            ConversationId = message.ConversationId,
            SenderUserId = message.SenderUserId,
            ContentType = message.ContentType,
            Text = message.Text,
            PayloadJson = message.PayloadJson,
            CreatedAtUtc = message.CreatedAtUtc,
            EditedAtUtc = message.EditedAtUtc,
            DeletedAtUtc = message.DeletedAtUtc,
            DeletedByUserId = message.DeletedByUserId
        };
    }

    public static ChatMessage ToDomain(this ChatMessageRecord record)
    {
        return new ChatMessage(
            record.Id,
            record.ConversationId,
            record.SenderUserId,
            record.ContentType,
            record.Text,
            record.PayloadJson,
            record.CreatedAtUtc,
            record.EditedAtUtc,
            record.DeletedAtUtc,
            record.DeletedByUserId);
    }
}
