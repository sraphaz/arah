using Araponga.Application.Interfaces;
using Araponga.Domain.Chat;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Modules.Chat.Infrastructure.Postgres;

public sealed class PostgresChatMessageRepository : IChatMessageRepository
{
    private readonly ChatDbContext _dbContext;

    public PostgresChatMessageRepository(ChatDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ChatMessage?> GetByIdAsync(Guid messageId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.ChatMessages
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == messageId, cancellationToken);

        return record?.ToDomain();
    }

    public async Task<IReadOnlyList<ChatMessage>> ListByConversationAsync(
        Guid conversationId,
        DateTime? beforeCreatedAtUtc,
        Guid? beforeMessageId,
        int limit,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.ChatMessages
            .AsNoTracking()
            .Where(m => m.ConversationId == conversationId);

        if (beforeCreatedAtUtc.HasValue && beforeMessageId.HasValue)
        {
            query = query.Where(m =>
                m.CreatedAtUtc < beforeCreatedAtUtc.Value ||
                (m.CreatedAtUtc == beforeCreatedAtUtc.Value && m.Id.CompareTo(beforeMessageId.Value) < 0));
        }

        var records = await query
            .OrderByDescending(m => m.CreatedAtUtc)
            .ThenByDescending(m => m.Id)
            .Take(limit)
            .ToListAsync(cancellationToken);

        return records.Select(r => r.ToDomain()).ToList();
    }

    public Task AddAsync(ChatMessage message, CancellationToken cancellationToken)
    {
        _dbContext.ChatMessages.Add(message.ToRecord());
        return Task.CompletedTask;
    }
}
