using Araponga.Domain.Chat;

namespace Araponga.Application.Interfaces;

public interface IChatConversationRepository
{
    Task<ChatConversation?> GetByIdAsync(Guid conversationId, CancellationToken cancellationToken);

    Task<ChatConversation?> GetTerritoryChannelAsync(
        Guid territoryId,
        ConversationKind kind,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<ChatConversation>> ListGroupsAsync(
        Guid territoryId,
        ConversationStatus status,
        CancellationToken cancellationToken);

    Task AddAsync(ChatConversation conversation, CancellationToken cancellationToken);
    Task UpdateAsync(ChatConversation conversation, CancellationToken cancellationToken);
}

