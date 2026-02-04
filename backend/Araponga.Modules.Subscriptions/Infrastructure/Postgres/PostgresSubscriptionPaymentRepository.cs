using Araponga.Application.Interfaces;
using Araponga.Domain.Subscriptions;
using Araponga.Modules.Subscriptions.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Modules.Subscriptions.Infrastructure.Postgres;

public sealed class PostgresSubscriptionPaymentRepository : ISubscriptionPaymentRepository
{
    private readonly SubscriptionsDbContext _dbContext;

    public PostgresSubscriptionPaymentRepository(SubscriptionsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SubscriptionPayment?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var record = await _dbContext.SubscriptionPayments
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        return record?.ToDomain();
    }

    public async Task<IReadOnlyList<SubscriptionPayment>> GetBySubscriptionIdAsync(Guid subscriptionId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.SubscriptionPayments
            .AsNoTracking()
            .Where(p => p.SubscriptionId == subscriptionId)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<SubscriptionPayment>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.SubscriptionPayments
            .AsNoTracking()
            .Join(_dbContext.Subscriptions,
                payment => payment.SubscriptionId,
                subscription => subscription.Id,
                (payment, subscription) => new { Payment = payment, Subscription = subscription })
            .Where(x => x.Subscription.UserId == userId)
            .Select(x => x.Payment)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public async Task<SubscriptionPayment?> GetByStripeInvoiceIdAsync(string stripeInvoiceId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.SubscriptionPayments
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.StripeInvoiceId == stripeInvoiceId, cancellationToken);
        return record?.ToDomain();
    }

    public async Task<SubscriptionPayment?> GetBySubscriptionAndPeriodAsync(
        Guid subscriptionId,
        DateTime periodStart,
        DateTime periodEnd,
        CancellationToken cancellationToken)
    {
        var record = await _dbContext.SubscriptionPayments
            .AsNoTracking()
            .FirstOrDefaultAsync(p =>
                p.SubscriptionId == subscriptionId &&
                p.PeriodStart == periodStart &&
                p.PeriodEnd == periodEnd,
                cancellationToken);
        return record?.ToDomain();
    }

    public async Task<IReadOnlyList<SubscriptionPayment>> GetByDateRangeAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken)
    {
        var records = await _dbContext.SubscriptionPayments
            .AsNoTracking()
            .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public Task AddAsync(SubscriptionPayment payment, CancellationToken cancellationToken)
    {
        _dbContext.SubscriptionPayments.Add(payment.ToRecord());
        return Task.CompletedTask;
    }

    public async Task UpdateAsync(SubscriptionPayment payment, CancellationToken cancellationToken)
    {
        var record = await _dbContext.SubscriptionPayments
            .FirstOrDefaultAsync(p => p.Id == payment.Id, cancellationToken);

        if (record == null)
        {
            throw new InvalidOperationException($"Subscription payment {payment.Id} not found.");
        }

        record.Status = (int)payment.Status;
        record.PaymentDate = payment.PaymentDate;
        record.PeriodStart = payment.PeriodStart;
        record.PeriodEnd = payment.PeriodEnd;
        record.StripeInvoiceId = payment.StripeInvoiceId;
        record.StripePaymentIntentId = payment.StripePaymentIntentId;
        record.FailureReason = payment.FailureReason;
        record.UpdatedAtUtc = payment.UpdatedAtUtc;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.SubscriptionPayments
            .AnyAsync(p => p.Id == id, cancellationToken);
    }
}
