namespace Araponga.Domain.Chat;

/// <summary>
/// Estados operacionais de uma conversa.
/// </summary>
public enum ConversationStatus
{
    Active = 1,
    PendingApproval = 2,
    Locked = 3,
    Disabled = 4
}
