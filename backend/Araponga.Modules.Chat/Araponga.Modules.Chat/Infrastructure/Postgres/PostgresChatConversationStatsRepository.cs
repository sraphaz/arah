using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Modules.Chat.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Modules.Chat.Infrastructure.Postgres;

public sealed class PostgresChatConversationStatsRepository : IChatConversationStatsRepository
{
    private readonly ChatDbContext _dbContext;

    public PostgresChatConversationStatsRepository(ChatDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ChatConversationStats?> GetAsync(Guid conversationId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.ChatConversationStats
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.ConversationId == conversationId, cancellationToken);

        return record is null
            ? null
            : new ChatConversationStats(
                record.ConversationId,
                record.LastMessageId,
                record.LastMessageAtUtc,
                record.LastSenderUserId,
                record.LastPreview,
                record.MessageCount);
    }

    public async Task UpsertAsync(ChatConversationStats stats, CancellationToken cancellationToken)
    {
        var record = await _dbContext.ChatConversationStats
            .FirstOrDefaultAsync(s => s.ConversationId == stats.ConversationId, cancellationToken);

        if (record is null)
        {
            _dbContext.ChatConversationStats.Add(new ChatConversationStatsRecord
            {
                ConversationId = stats.ConversationId,
                LastMessageId = stats.LastMessageId,
                LastMessageAtUtc = stats.LastMessageAtUtc,
                LastSenderUserId = stats.LastSenderUserId,
                LastPreview = stats.LastPreview,
                MessageCount = stats.MessageCount
            });
            return;
        }

        record.LastMessageId = stats.LastMessageId;
        record.LastMessageAtUtc = stats.LastMessageAtUtc;
        record.LastSenderUserId = stats.LastSenderUserId;
        record.LastPreview = stats.LastPreview;
        record.MessageCount = stats.MessageCount;
    }
}
