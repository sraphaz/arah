using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Infrastructure.Postgres.Entities;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresOutbox : IOutbox
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresOutbox(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task EnqueueAsync(OutboxMessage message, CancellationToken cancellationToken)
    {
        _dbContext.OutboxMessages.Add(new OutboxMessageRecord
        {
            Id = message.Id,
            Type = message.Type,
            PayloadJson = message.PayloadJson,
            OccurredAtUtc = message.OccurredAtUtc,
            ProcessedAtUtc = message.ProcessedAtUtc,
            Attempts = message.Attempts,
            LastError = message.LastError,
            ProcessAfterUtc = message.ProcessAfterUtc
        });

        return Task.CompletedTask;
    }
}
