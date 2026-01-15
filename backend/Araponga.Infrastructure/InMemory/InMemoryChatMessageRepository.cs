using Araponga.Application.Interfaces;
using Araponga.Domain.Chat;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryChatMessageRepository : IChatMessageRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryChatMessageRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<ChatMessage?> GetByIdAsync(Guid messageId, CancellationToken cancellationToken)
    {
        var message = _dataStore.ChatMessages.FirstOrDefault(m => m.Id == messageId);
        return Task.FromResult(message);
    }

    public Task<IReadOnlyList<ChatMessage>> ListByConversationAsync(
        Guid conversationId,
        DateTime? beforeCreatedAtUtc,
        Guid? beforeMessageId,
        int limit,
        CancellationToken cancellationToken)
    {
        IEnumerable<ChatMessage> query = _dataStore.ChatMessages.Where(m => m.ConversationId == conversationId);

        if (beforeCreatedAtUtc.HasValue && beforeMessageId.HasValue)
        {
            query = query.Where(m =>
                m.CreatedAtUtc < beforeCreatedAtUtc.Value ||
                (m.CreatedAtUtc == beforeCreatedAtUtc.Value && m.Id.CompareTo(beforeMessageId.Value) < 0));
        }

        var page = query
            .OrderByDescending(m => m.CreatedAtUtc)
            .ThenByDescending(m => m.Id)
            .Take(limit)
            .ToList();

        return Task.FromResult<IReadOnlyList<ChatMessage>>(page);
    }

    public Task AddAsync(ChatMessage message, CancellationToken cancellationToken)
    {
        _dataStore.ChatMessages.Add(message);
        return Task.CompletedTask;
    }
}

