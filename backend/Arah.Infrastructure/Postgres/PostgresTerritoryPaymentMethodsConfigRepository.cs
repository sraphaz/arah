using Arah.Application.Interfaces;
using Arah.Domain.Fiscal;
using Microsoft.EntityFrameworkCore;

namespace Arah.Infrastructure.Postgres;

public sealed class PostgresTerritoryPaymentMethodsConfigRepository : ITerritoryPaymentMethodsConfigRepository
{
    private readonly ArahDbContext _dbContext;

    public PostgresTerritoryPaymentMethodsConfigRepository(ArahDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TerritoryPaymentMethodsConfig?> GetByTerritoryIdAsync(
        Guid territoryId,
        CancellationToken cancellationToken)
    {
        var record = await _dbContext.TerritoryPaymentMethodsConfigs
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.TerritoryId == territoryId, cancellationToken);
        return record?.ToDomain();
    }

    public async Task AddAsync(TerritoryPaymentMethodsConfig config, CancellationToken cancellationToken)
    {
        var exists = await _dbContext.TerritoryPaymentMethodsConfigs
            .AnyAsync(c => c.TerritoryId == config.TerritoryId, cancellationToken);
        if (exists)
        {
            throw new InvalidOperationException(
                $"Payment methods config already exists for territory {config.TerritoryId}.");
        }

        _dbContext.TerritoryPaymentMethodsConfigs.Add(config.ToRecord());
    }

    public async Task UpdateAsync(TerritoryPaymentMethodsConfig config, CancellationToken cancellationToken)
    {
        var record = await _dbContext.TerritoryPaymentMethodsConfigs
            .FirstOrDefaultAsync(c => c.Id == config.Id, cancellationToken)
            ?? throw new InvalidOperationException($"TerritoryPaymentMethodsConfig {config.Id} not found.");

        record.MethodsCsv = string.Join(',', config.Methods.Select(m => m.ToString()));
        record.PspProvider = config.PspProvider;
        record.UpdatedAtUtc = config.UpdatedAtUtc;
    }
}
