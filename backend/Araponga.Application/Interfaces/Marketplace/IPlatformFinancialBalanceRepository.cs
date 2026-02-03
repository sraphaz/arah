using Araponga.Domain.Financial;

namespace Araponga.Application.Interfaces;

public interface IPlatformFinancialBalanceRepository
{
    Task<PlatformFinancialBalance?> GetByTerritoryIdAsync(Guid territoryId, CancellationToken cancellationToken);
    Task<List<PlatformFinancialBalance>> GetAllAsync(CancellationToken cancellationToken);
    Task AddAsync(PlatformFinancialBalance balance, CancellationToken cancellationToken);
    Task UpdateAsync(PlatformFinancialBalance balance, CancellationToken cancellationToken);
}
