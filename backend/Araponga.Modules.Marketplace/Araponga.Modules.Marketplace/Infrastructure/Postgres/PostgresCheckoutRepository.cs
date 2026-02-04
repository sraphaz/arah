using Araponga.Modules.Marketplace.Application.Interfaces;
using Araponga.Modules.Marketplace.Domain;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Modules.Marketplace.Infrastructure.Postgres;

public sealed class PostgresCheckoutRepository : ICheckoutRepository
{
    private readonly MarketplaceDbContext _dbContext;

    public PostgresCheckoutRepository(MarketplaceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Checkout?> GetByIdAsync(Guid checkoutId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.Checkouts
            .FirstOrDefaultAsync(c => c.Id == checkoutId, cancellationToken);
        return record?.ToDomain();
    }

    public Task AddAsync(Checkout checkout, CancellationToken cancellationToken)
    {
        _dbContext.Checkouts.Add(checkout.ToRecord());
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Checkout checkout, CancellationToken cancellationToken)
    {
        _dbContext.Checkouts.Update(checkout.ToRecord());
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<Checkout>> ListByUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.Checkouts
            .AsNoTracking()
            .Where(c => c.BuyerUserId == userId)
            .OrderByDescending(c => c.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return records.Select(record => record.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<Checkout>> ListAllAsync(CancellationToken cancellationToken)
    {
        var records = await _dbContext.Checkouts
            .AsNoTracking()
            .OrderByDescending(c => c.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return records.Select(record => record.ToDomain()).ToList();
    }
}
