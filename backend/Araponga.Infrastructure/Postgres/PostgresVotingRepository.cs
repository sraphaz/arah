using Araponga.Application.Interfaces;
using Araponga.Domain.Governance;
using Araponga.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresVotingRepository : IVotingRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresVotingRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(Voting voting, CancellationToken cancellationToken)
    {
        _dbContext.Votings.Add(voting.ToRecord());
        return Task.CompletedTask;
    }

    public async Task UpdateAsync(Voting voting, CancellationToken cancellationToken)
    {
        var record = await _dbContext.Votings
            .FirstOrDefaultAsync(v => v.Id == voting.Id, cancellationToken);

        if (record is null)
        {
            _dbContext.Votings.Add(voting.ToRecord());
        }
        else
        {
            var updatedRecord = voting.ToRecord();
            _dbContext.Entry(record).CurrentValues.SetValues(updatedRecord);
        }
    }

    public async Task<Voting?> GetByIdAsync(Guid votingId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.Votings
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == votingId, cancellationToken);

        return record?.ToDomain();
    }

    public async Task<IReadOnlyList<Voting>> ListByTerritoryAsync(
        Guid territoryId,
        VotingStatus? status,
        Guid? createdByUserId,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Votings
            .AsNoTracking()
            .Where(v => v.TerritoryId == territoryId);

        if (status.HasValue)
        {
            query = query.Where(v => v.Status == status.Value);
        }

        if (createdByUserId.HasValue)
        {
            query = query.Where(v => v.CreatedByUserId == createdByUserId.Value);
        }

        var records = await query
            .OrderByDescending(v => v.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return records.Select(r => r.ToDomain()).ToList();
    }
}
