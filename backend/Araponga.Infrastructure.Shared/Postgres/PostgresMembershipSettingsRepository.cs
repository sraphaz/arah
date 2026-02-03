using Araponga.Application.Interfaces;
using Araponga.Domain.Membership;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Shared.Postgres;

/// <summary>Implementação Postgres de IMembershipSettingsRepository usando SharedDbContext.</summary>
public sealed class PostgresMembershipSettingsRepository : IMembershipSettingsRepository
{
    private readonly SharedDbContext _dbContext;

    public PostgresMembershipSettingsRepository(SharedDbContext dbContext) => _dbContext = dbContext;

    public async Task<MembershipSettings?> GetByMembershipIdAsync(Guid membershipId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.MembershipSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.MembershipId == membershipId, cancellationToken);
        return record?.ToDomain();
    }

    public Task AddAsync(MembershipSettings settings, CancellationToken cancellationToken)
    {
        _dbContext.MembershipSettings.Add(settings.ToRecord());
        return Task.CompletedTask;
    }

    public async Task UpdateAsync(MembershipSettings settings, CancellationToken cancellationToken)
    {
        var record = await _dbContext.MembershipSettings
            .FirstOrDefaultAsync(s => s.MembershipId == settings.MembershipId, cancellationToken);
        if (record is null)
            _dbContext.MembershipSettings.Add(settings.ToRecord());
        else
            _dbContext.Entry(record).CurrentValues.SetValues(settings.ToRecord());
    }
}
