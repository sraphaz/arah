using Araponga.Application.Interfaces;
using Araponga.Domain.Marketplace;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresCartItemRepository : ICartItemRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresCartItemRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<CartItem>> ListByCartIdAsync(Guid cartId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.CartItems
            .AsNoTracking()
            .Where(i => i.CartId == cartId)
            .ToListAsync(cancellationToken);

        return records.Select(record => record.ToDomain()).ToList();
    }

    public async Task<CartItem?> GetByIdAsync(Guid cartItemId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.CartItems
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == cartItemId, cancellationToken);

        return record?.ToDomain();
    }

    public async Task<CartItem?> GetByCartAndListingAsync(Guid cartId, Guid listingId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.CartItems
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.CartId == cartId && i.ListingId == listingId, cancellationToken);

        return record?.ToDomain();
    }

    public Task AddAsync(CartItem item, CancellationToken cancellationToken)
    {
        _dbContext.CartItems.Add(item.ToRecord());
        return Task.CompletedTask;
    }

    public Task UpdateAsync(CartItem item, CancellationToken cancellationToken)
    {
        _dbContext.CartItems.Update(item.ToRecord());
        return Task.CompletedTask;
    }

    public async Task RemoveAsync(Guid cartItemId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.CartItems
            .FirstOrDefaultAsync(i => i.Id == cartItemId, cancellationToken);

        if (record is null)
        {
            return;
        }

        _dbContext.CartItems.Remove(record);
    }

    public async Task RemoveByCartIdAsync(Guid cartId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.CartItems
            .Where(i => i.CartId == cartId)
            .ToListAsync(cancellationToken);

        if (records.Count == 0)
        {
            return;
        }

        _dbContext.CartItems.RemoveRange(records);
    }
}
