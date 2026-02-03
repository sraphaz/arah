using Araponga.Application.Interfaces;
using Araponga.Domain.Governance;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Shared.Postgres;

/// <summary>Implementação Postgres de IVoteRepository usando SharedDbContext.</summary>
public sealed class PostgresVoteRepository : IVoteRepository
{
    private readonly SharedDbContext _dbContext;

    public PostgresVoteRepository(SharedDbContext dbContext) => _dbContext = dbContext;

    public Task AddAsync(Vote vote, CancellationToken cancellationToken)
    {
        _dbContext.Votes.Add(vote.ToRecord());
        return Task.CompletedTask;
    }

    public async Task<bool> HasVotedAsync(Guid votingId, Guid userId, CancellationToken cancellationToken)
    {
        return await _dbContext.Votes
            .AsNoTracking()
            .AnyAsync(v => v.VotingId == votingId && v.UserId == userId, cancellationToken);
    }

    public async Task<IReadOnlyList<Vote>> ListByVotingIdAsync(Guid votingId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.Votes
            .AsNoTracking()
            .Where(v => v.VotingId == votingId)
            .OrderBy(v => v.CreatedAtUtc)
            .ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public async Task<Dictionary<string, int>> CountByOptionAsync(Guid votingId, CancellationToken cancellationToken)
    {
        var counts = await _dbContext.Votes
            .AsNoTracking()
            .Where(v => v.VotingId == votingId)
            .GroupBy(v => v.SelectedOption)
            .Select(g => new { Option = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);
        return counts.ToDictionary(c => c.Option, c => c.Count);
    }

    public async Task<IReadOnlyList<Vote>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.Votes
            .AsNoTracking()
            .Where(v => v.UserId == userId)
            .OrderByDescending(v => v.CreatedAtUtc)
            .ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }
}
