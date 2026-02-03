using Araponga.Application.Interfaces;
using Araponga.Domain.Marketplace;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Modules.Marketplace.Infrastructure.Postgres;

public sealed class PostgresCartRepository : ICartRepository
{
    private readonly MarketplaceDbContext _dbContext;

    public PostgresCartRepository(MarketplaceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Cart?> GetByUserAsync(Guid territoryId, Guid userId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.Carts
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.TerritoryId == territoryId && c.UserId == userId, cancellationToken);

        return record?.ToDomain();
    }

    public async Task<Cart?> GetByIdAsync(Guid cartId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.Carts
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == cartId, cancellationToken);

        return record?.ToDomain();
    }

    public Task AddAsync(Cart cart, CancellationToken cancellationToken)
    {
        _dbContext.Carts.Add(cart.ToRecord());
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Cart cart, CancellationToken cancellationToken)
    {
        _dbContext.Carts.Update(cart.ToRecord());
        return Task.CompletedTask;
    }
}
