using Araponga.Application.Interfaces;
using Araponga.Domain.Subscriptions;
using Araponga.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresCouponRepository : ICouponRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresCouponRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Coupon?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var record = await _dbContext.Coupons
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        return record?.ToDomain();
    }

    public async Task<Coupon?> GetByCodeAsync(string code, CancellationToken cancellationToken)
    {
        var record = await _dbContext.Coupons
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Code == code.ToUpperInvariant(), cancellationToken);
        return record?.ToDomain();
    }

    public async Task<IReadOnlyList<Coupon>> ListAsync(bool? isActive, CancellationToken cancellationToken)
    {
        var query = _dbContext.Coupons.AsNoTracking().AsQueryable();

        if (isActive.HasValue)
        {
            query = query.Where(c => c.IsActive == isActive.Value);
        }

        var records = await query.ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public Task AddAsync(Coupon coupon, CancellationToken cancellationToken)
    {
        _dbContext.Coupons.Add(coupon.ToRecord());
        return Task.CompletedTask;
    }

    public async Task UpdateAsync(Coupon coupon, CancellationToken cancellationToken)
    {
        var record = await _dbContext.Coupons
            .FirstOrDefaultAsync(c => c.Id == coupon.Id, cancellationToken);

        if (record == null)
        {
            throw new InvalidOperationException($"Coupon {coupon.Id} not found.");
        }

        record.Code = coupon.Code;
        record.Name = coupon.Name;
        record.Description = coupon.Description;
        record.DiscountType = (int)coupon.DiscountType;
        record.DiscountValue = coupon.DiscountValue;
        record.ValidFrom = coupon.ValidFrom;
        record.ValidUntil = coupon.ValidUntil;
        record.MaxUses = coupon.MaxUses;
        record.UsedCount = coupon.UsedCount;
        record.IsActive = coupon.IsActive;
        record.StripeCouponId = coupon.StripeCouponId;
        record.UpdatedAtUtc = coupon.UpdatedAtUtc;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Coupons
            .AnyAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken)
    {
        return await _dbContext.Coupons
            .AnyAsync(c => c.Code == code.ToUpperInvariant(), cancellationToken);
    }
}
