namespace Araponga.Domain.Chat;

public sealed class ChatMessage
{
    public const int MaxTextLength = 5_000;
    public const int MaxPayloadLength = 20_000;

    public ChatMessage(
        Guid id,
        Guid conversationId,
        Guid senderUserId,
        MessageContentType contentType,
        string? text,
        string? payloadJson,
        DateTime createdAtUtc,
        DateTime? editedAtUtc,
        DateTime? deletedAtUtc,
        Guid? deletedByUserId)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("ID is required.", nameof(id));
        }

        if (conversationId == Guid.Empty)
        {
            throw new ArgumentException("ConversationId is required.", nameof(conversationId));
        }

        if (senderUserId == Guid.Empty)
        {
            throw new ArgumentException("SenderUserId is required.", nameof(senderUserId));
        }

        var normalizedText = string.IsNullOrWhiteSpace(text) ? null : text.Trim();
        if (contentType == MessageContentType.Text)
        {
            if (string.IsNullOrWhiteSpace(normalizedText))
            {
                throw new ArgumentException("Text is required for Text messages.", nameof(text));
            }
        }

        if (normalizedText is not null && normalizedText.Length > MaxTextLength)
        {
            throw new ArgumentException($"Text must not exceed {MaxTextLength} characters.", nameof(text));
        }

        var normalizedPayload = string.IsNullOrWhiteSpace(payloadJson) ? null : payloadJson.Trim();
        if (normalizedPayload is not null && normalizedPayload.Length > MaxPayloadLength)
        {
            throw new ArgumentException($"Payload must not exceed {MaxPayloadLength} characters.", nameof(payloadJson));
        }

        if (deletedAtUtc is not null && (deletedByUserId is null || deletedByUserId.Value == Guid.Empty))
        {
            throw new ArgumentException("DeletedByUserId is required when DeletedAtUtc is set.", nameof(deletedByUserId));
        }

        Id = id;
        ConversationId = conversationId;
        SenderUserId = senderUserId;
        ContentType = contentType;
        Text = normalizedText;
        PayloadJson = normalizedPayload;
        CreatedAtUtc = createdAtUtc;
        EditedAtUtc = editedAtUtc;
        DeletedAtUtc = deletedAtUtc;
        DeletedByUserId = deletedByUserId;
    }

    public Guid Id { get; }
    public Guid ConversationId { get; }
    public Guid SenderUserId { get; }
    public MessageContentType ContentType { get; }
    public string? Text { get; private set; }
    public string? PayloadJson { get; private set; }
    public DateTime CreatedAtUtc { get; }
    public DateTime? EditedAtUtc { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }
    public Guid? DeletedByUserId { get; private set; }

    public void Edit(string newText, DateTime editedAtUtc)
    {
        if (ContentType != MessageContentType.Text)
        {
            throw new InvalidOperationException("Only text messages can be edited.");
        }

        if (string.IsNullOrWhiteSpace(newText))
        {
            throw new ArgumentException("Text is required.", nameof(newText));
        }

        var normalized = newText.Trim();
        if (normalized.Length > MaxTextLength)
        {
            throw new ArgumentException($"Text must not exceed {MaxTextLength} characters.", nameof(newText));
        }

        if (DeletedAtUtc is not null)
        {
            throw new InvalidOperationException("Cannot edit a deleted message.");
        }

        Text = normalized;
        EditedAtUtc = editedAtUtc;
    }

    public void Delete(Guid deletedByUserId, DateTime deletedAtUtc)
    {
        if (deletedByUserId == Guid.Empty)
        {
            throw new ArgumentException("DeletedByUserId is required.", nameof(deletedByUserId));
        }

        if (DeletedAtUtc is not null)
        {
            return;
        }

        DeletedByUserId = deletedByUserId;
        DeletedAtUtc = deletedAtUtc;
    }
}
