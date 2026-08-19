using Arah.Domain.Fiscal;

namespace Arah.Application.Interfaces;

public interface ITerritoryFiscalPackBindingRepository
{
    Task<TerritoryFiscalPackBinding?> GetByTerritoryIdAsync(Guid territoryId, CancellationToken cancellationToken);
    Task AddAsync(TerritoryFiscalPackBinding binding, CancellationToken cancellationToken);
    Task UpdateAsync(TerritoryFiscalPackBinding binding, CancellationToken cancellationToken);
}
