namespace Araponga.Domain.Chat;

/// <summary>
/// Papel do usuário dentro da conversa (independente de VISITOR/RESIDENT).
/// </summary>
public enum ConversationParticipantRole
{
    Member = 1,
    Owner = 2
}
