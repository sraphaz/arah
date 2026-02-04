using Araponga.Application.Interfaces;
using Araponga.Domain.Chat;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Modules.Chat.Infrastructure.Postgres;

public sealed class PostgresChatConversationParticipantRepository : IChatConversationParticipantRepository
{
    private readonly ChatDbContext _dbContext;

    public PostgresChatConversationParticipantRepository(ChatDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ConversationParticipant?> GetAsync(Guid conversationId, Guid userId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.ChatConversationParticipants
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == userId, cancellationToken);

        return record?.ToDomain();
    }

    public async Task<IReadOnlyList<ConversationParticipant>> ListByConversationAsync(Guid conversationId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.ChatConversationParticipants
            .AsNoTracking()
            .Where(p => p.ConversationId == conversationId)
            .ToListAsync(cancellationToken);

        return records.Select(r => r.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<Guid>> ListConversationIdsByUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var ids = await _dbContext.ChatConversationParticipants
            .AsNoTracking()
            .Where(p => p.UserId == userId && p.LeftAtUtc == null)
            .Select(p => p.ConversationId)
            .ToListAsync(cancellationToken);

        return ids;
    }

    public Task AddAsync(ConversationParticipant participant, CancellationToken cancellationToken)
    {
        _dbContext.ChatConversationParticipants.Add(participant.ToRecord());
        return Task.CompletedTask;
    }

    public async Task UpdateAsync(ConversationParticipant participant, CancellationToken cancellationToken)
    {
        var record = await _dbContext.ChatConversationParticipants
            .FirstOrDefaultAsync(p => p.ConversationId == participant.ConversationId && p.UserId == participant.UserId, cancellationToken);

        if (record is null)
        {
            return;
        }

        record.Role = participant.Role;
        record.LeftAtUtc = participant.LeftAtUtc;
        record.MutedUntilUtc = participant.MutedUntilUtc;
        record.LastReadMessageId = participant.LastReadMessageId;
        record.LastReadAtUtc = participant.LastReadAtUtc;
    }

    public async Task RemoveAsync(Guid conversationId, Guid userId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.ChatConversationParticipants
            .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == userId, cancellationToken);

        if (record is null)
        {
            return;
        }

        _dbContext.ChatConversationParticipants.Remove(record);
    }
}
