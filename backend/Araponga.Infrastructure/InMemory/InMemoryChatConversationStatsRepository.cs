using Araponga.Application.Interfaces;
using Araponga.Application.Models;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryChatConversationStatsRepository : IChatConversationStatsRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryChatConversationStatsRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<ChatConversationStats?> GetAsync(Guid conversationId, CancellationToken cancellationToken)
    {
        var stats = _dataStore.ChatStats.FirstOrDefault(s => s.ConversationId == conversationId);
        return Task.FromResult(stats);
    }

    public Task UpsertAsync(ChatConversationStats stats, CancellationToken cancellationToken)
    {
        _dataStore.ChatStats.RemoveAll(s => s.ConversationId == stats.ConversationId);
        _dataStore.ChatStats.Add(stats);
        return Task.CompletedTask;
    }
}

