using Araponga.Domain.Chat;

namespace Araponga.Application.Interfaces;

public interface IChatMessageRepository
{
    Task<ChatMessage?> GetByIdAsync(Guid messageId, CancellationToken cancellationToken);

    /// <summary>
    /// Lista mensagens da conversa antes de um cursor (createdAtUtc + id), ordenadas do mais recente para o mais antigo.
    /// Se beforeCreatedAtUtc/beforeMessageId forem nulos, retorna a página mais recente.
    /// </summary>
    Task<IReadOnlyList<ChatMessage>> ListByConversationAsync(
        Guid conversationId,
        DateTime? beforeCreatedAtUtc,
        Guid? beforeMessageId,
        int limit,
        CancellationToken cancellationToken);

    Task AddAsync(ChatMessage message, CancellationToken cancellationToken);
}
