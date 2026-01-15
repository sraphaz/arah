namespace Araponga.Domain.Chat;

public sealed class ChatConversation
{
    public const int MaxNameLength = 120;

    public ChatConversation(
        Guid id,
        ConversationKind kind,
        ConversationStatus status,
        Guid? territoryId,
        string? name,
        Guid createdByUserId,
        DateTime createdAtUtc,
        Guid? approvedByUserId,
        DateTime? approvedAtUtc,
        DateTime? lockedAtUtc,
        Guid? lockedByUserId,
        DateTime? disabledAtUtc,
        Guid? disabledByUserId)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("ID is required.", nameof(id));
        }

        if (createdByUserId == Guid.Empty)
        {
            throw new ArgumentException("CreatedByUserId is required.", nameof(createdByUserId));
        }

        // Conversas territoriais precisam de TerritoryId
        if (kind is ConversationKind.TerritoryPublic or ConversationKind.TerritoryResidents or ConversationKind.Group)
        {
            if (territoryId is null || territoryId.Value == Guid.Empty)
            {
                throw new ArgumentException("TerritoryId is required for territory-scoped conversations.", nameof(territoryId));
            }
        }

        // DM não deve estar acoplada a território
        if (kind == ConversationKind.Direct && territoryId is not null)
        {
            throw new ArgumentException("TerritoryId must be null for Direct conversations.", nameof(territoryId));
        }

        var normalizedName = string.IsNullOrWhiteSpace(name) ? null : name.Trim();
        if (normalizedName is not null && normalizedName.Length > MaxNameLength)
        {
            throw new ArgumentException($"Name must not exceed {MaxNameLength} characters.", nameof(name));
        }

        // Regras de consistência de status
        if (status == ConversationStatus.Active && kind == ConversationKind.Group && approvedAtUtc is null)
        {
            // Grupo ativo implica que houve habilitação/aprovação.
            // (Para channels, a criação pode ser ativa sem aprovação.)
            throw new ArgumentException("ApprovedAtUtc is required when a Group is Active.", nameof(approvedAtUtc));
        }

        if (approvedAtUtc is not null && (approvedByUserId is null || approvedByUserId.Value == Guid.Empty))
        {
            throw new ArgumentException("ApprovedByUserId is required when ApprovedAtUtc is set.", nameof(approvedByUserId));
        }

        if (lockedAtUtc is not null && (lockedByUserId is null || lockedByUserId.Value == Guid.Empty))
        {
            throw new ArgumentException("LockedByUserId is required when LockedAtUtc is set.", nameof(lockedByUserId));
        }

        if (disabledAtUtc is not null && (disabledByUserId is null || disabledByUserId.Value == Guid.Empty))
        {
            throw new ArgumentException("DisabledByUserId is required when DisabledAtUtc is set.", nameof(disabledByUserId));
        }

        Id = id;
        Kind = kind;
        Status = status;
        TerritoryId = territoryId;
        Name = normalizedName;
        CreatedByUserId = createdByUserId;
        CreatedAtUtc = createdAtUtc;
        ApprovedByUserId = approvedByUserId;
        ApprovedAtUtc = approvedAtUtc;
        LockedAtUtc = lockedAtUtc;
        LockedByUserId = lockedByUserId;
        DisabledAtUtc = disabledAtUtc;
        DisabledByUserId = disabledByUserId;
    }

    public Guid Id { get; }
    public ConversationKind Kind { get; }
    public ConversationStatus Status { get; private set; }
    public Guid? TerritoryId { get; }
    public string? Name { get; private set; }

    public Guid CreatedByUserId { get; }
    public DateTime CreatedAtUtc { get; }

    public Guid? ApprovedByUserId { get; private set; }
    public DateTime? ApprovedAtUtc { get; private set; }

    public DateTime? LockedAtUtc { get; private set; }
    public Guid? LockedByUserId { get; private set; }

    public DateTime? DisabledAtUtc { get; private set; }
    public Guid? DisabledByUserId { get; private set; }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name is required.", nameof(name));
        }

        var normalized = name.Trim();
        if (normalized.Length > MaxNameLength)
        {
            throw new ArgumentException($"Name must not exceed {MaxNameLength} characters.", nameof(name));
        }

        Name = normalized;
    }

    public void Approve(Guid approvedByUserId, DateTime approvedAtUtc)
    {
        if (approvedByUserId == Guid.Empty)
        {
            throw new ArgumentException("ApprovedByUserId is required.", nameof(approvedByUserId));
        }

        if (Status != ConversationStatus.PendingApproval)
        {
            throw new InvalidOperationException("Only pending conversations can be approved.");
        }

        if (Kind != ConversationKind.Group)
        {
            throw new InvalidOperationException("Only Group conversations require approval.");
        }

        Status = ConversationStatus.Active;
        ApprovedByUserId = approvedByUserId;
        ApprovedAtUtc = approvedAtUtc;
        DisabledAtUtc = null;
        DisabledByUserId = null;
    }

    public void Lock(Guid lockedByUserId, DateTime lockedAtUtc)
    {
        if (lockedByUserId == Guid.Empty)
        {
            throw new ArgumentException("LockedByUserId is required.", nameof(lockedByUserId));
        }

        if (Status == ConversationStatus.Disabled)
        {
            throw new InvalidOperationException("Cannot lock a disabled conversation.");
        }

        Status = ConversationStatus.Locked;
        LockedByUserId = lockedByUserId;
        LockedAtUtc = lockedAtUtc;
    }

    public void Unlock()
    {
        if (Status != ConversationStatus.Locked)
        {
            throw new InvalidOperationException("Conversation is not locked.");
        }

        Status = ConversationStatus.Active;
        LockedAtUtc = null;
        LockedByUserId = null;
    }

    public void Disable(Guid disabledByUserId, DateTime disabledAtUtc)
    {
        if (disabledByUserId == Guid.Empty)
        {
            throw new ArgumentException("DisabledByUserId is required.", nameof(disabledByUserId));
        }

        if (Status == ConversationStatus.Disabled)
        {
            return;
        }

        Status = ConversationStatus.Disabled;
        DisabledByUserId = disabledByUserId;
        DisabledAtUtc = disabledAtUtc;
    }
}

