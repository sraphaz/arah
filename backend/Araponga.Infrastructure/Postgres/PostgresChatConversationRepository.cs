using Araponga.Application.Interfaces;
using Araponga.Domain.Chat;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresChatConversationRepository : IChatConversationRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresChatConversationRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ChatConversation?> GetByIdAsync(Guid conversationId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.ChatConversations
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == conversationId, cancellationToken);

        return record?.ToDomain();
    }

    public async Task<ChatConversation?> GetTerritoryChannelAsync(
        Guid territoryId,
        ConversationKind kind,
        CancellationToken cancellationToken)
    {
        var record = await _dbContext.ChatConversations
            .AsNoTracking()
            .FirstOrDefaultAsync(
                c => c.TerritoryId == territoryId && c.Kind == kind,
                cancellationToken);

        return record?.ToDomain();
    }

    public async Task<IReadOnlyList<ChatConversation>> ListGroupsAsync(
        Guid territoryId,
        ConversationStatus status,
        CancellationToken cancellationToken)
    {
        var records = await _dbContext.ChatConversations
            .AsNoTracking()
            .Where(c => c.TerritoryId == territoryId &&
                        c.Kind == ConversationKind.Group &&
                        c.Status == status)
            .OrderByDescending(c => c.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return records.Select(r => r.ToDomain()).ToList();
    }

    public Task AddAsync(ChatConversation conversation, CancellationToken cancellationToken)
    {
        _dbContext.ChatConversations.Add(conversation.ToRecord());
        return Task.CompletedTask;
    }

    public async Task UpdateAsync(ChatConversation conversation, CancellationToken cancellationToken)
    {
        var record = await _dbContext.ChatConversations
            .FirstOrDefaultAsync(c => c.Id == conversation.Id, cancellationToken);

        if (record is null)
        {
            return;
        }

        record.Status = conversation.Status;
        record.Name = conversation.Name;
        record.ApprovedByUserId = conversation.ApprovedByUserId;
        record.ApprovedAtUtc = conversation.ApprovedAtUtc;
        record.LockedByUserId = conversation.LockedByUserId;
        record.LockedAtUtc = conversation.LockedAtUtc;
        record.DisabledByUserId = conversation.DisabledByUserId;
        record.DisabledAtUtc = conversation.DisabledAtUtc;
    }
}

