using Araponga.Application.Interfaces;
using Araponga.Domain.Chat;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryChatConversationRepository : IChatConversationRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryChatConversationRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<ChatConversation?> GetByIdAsync(Guid conversationId, CancellationToken cancellationToken)
    {
        var conversation = _dataStore.ChatConversations.FirstOrDefault(c => c.Id == conversationId);
        return Task.FromResult(conversation);
    }

    public Task<ChatConversation?> GetTerritoryChannelAsync(Guid territoryId, ConversationKind kind, CancellationToken cancellationToken)
    {
        var conversation = _dataStore.ChatConversations.FirstOrDefault(c =>
            c.TerritoryId == territoryId && c.Kind == kind);
        return Task.FromResult(conversation);
    }

    public Task<IReadOnlyList<ChatConversation>> ListGroupsAsync(Guid territoryId, ConversationStatus status, CancellationToken cancellationToken)
    {
        var groups = _dataStore.ChatConversations
            .Where(c => c.TerritoryId == territoryId && c.Kind == ConversationKind.Group && c.Status == status)
            .OrderByDescending(c => c.CreatedAtUtc)
            .ToList();

        return Task.FromResult<IReadOnlyList<ChatConversation>>(groups);
    }

    public Task AddAsync(ChatConversation conversation, CancellationToken cancellationToken)
    {
        _dataStore.ChatConversations.Add(conversation);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(ChatConversation conversation, CancellationToken cancellationToken)
    {
        // In-memory: referência já é a mesma; nada a fazer.
        return Task.CompletedTask;
    }
}

