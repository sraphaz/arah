using Araponga.Domain.Chat;

namespace Araponga.Application.Interfaces;

public interface IChatConversationParticipantRepository
{
    Task<ConversationParticipant?> GetAsync(Guid conversationId, Guid userId, CancellationToken cancellationToken);
    Task<IReadOnlyList<ConversationParticipant>> ListByConversationAsync(Guid conversationId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Guid>> ListConversationIdsByUserAsync(Guid userId, CancellationToken cancellationToken);
    Task AddAsync(ConversationParticipant participant, CancellationToken cancellationToken);
    Task UpdateAsync(ConversationParticipant participant, CancellationToken cancellationToken);
    Task RemoveAsync(Guid conversationId, Guid userId, CancellationToken cancellationToken);
}

