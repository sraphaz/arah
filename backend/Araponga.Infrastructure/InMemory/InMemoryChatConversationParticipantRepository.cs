using Araponga.Application.Interfaces;
using Araponga.Domain.Chat;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryChatConversationParticipantRepository : IChatConversationParticipantRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryChatConversationParticipantRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<ConversationParticipant?> GetAsync(Guid conversationId, Guid userId, CancellationToken cancellationToken)
    {
        var participant = _dataStore.ChatParticipants
            .FirstOrDefault(p => p.ConversationId == conversationId && p.UserId == userId);
        return Task.FromResult(participant);
    }

    public Task<IReadOnlyList<ConversationParticipant>> ListByConversationAsync(Guid conversationId, CancellationToken cancellationToken)
    {
        var participants = _dataStore.ChatParticipants
            .Where(p => p.ConversationId == conversationId)
            .ToList();
        return Task.FromResult<IReadOnlyList<ConversationParticipant>>(participants);
    }

    public Task<IReadOnlyList<Guid>> ListConversationIdsByUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var ids = _dataStore.ChatParticipants
            .Where(p => p.UserId == userId && p.LeftAtUtc is null)
            .Select(p => p.ConversationId)
            .Distinct()
            .ToList();

        return Task.FromResult<IReadOnlyList<Guid>>(ids);
    }

    public Task AddAsync(ConversationParticipant participant, CancellationToken cancellationToken)
    {
        _dataStore.ChatParticipants.Add(participant);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(ConversationParticipant participant, CancellationToken cancellationToken)
    {
        // In-memory: referência já é a mesma; nada a fazer.
        return Task.CompletedTask;
    }

    public Task RemoveAsync(Guid conversationId, Guid userId, CancellationToken cancellationToken)
    {
        _dataStore.ChatParticipants.RemoveAll(p => p.ConversationId == conversationId && p.UserId == userId);
        return Task.CompletedTask;
    }
}

