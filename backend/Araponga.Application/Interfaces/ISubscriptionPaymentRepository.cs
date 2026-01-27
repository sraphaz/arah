using Araponga.Domain.Subscriptions;

namespace Araponga.Application.Interfaces;

public interface ISubscriptionPaymentRepository
{
    Task<SubscriptionPayment?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<SubscriptionPayment>> GetBySubscriptionIdAsync(Guid subscriptionId, CancellationToken cancellationToken);
    Task<IReadOnlyList<SubscriptionPayment>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<SubscriptionPayment?> GetByStripeInvoiceIdAsync(string stripeInvoiceId, CancellationToken cancellationToken);
    Task<SubscriptionPayment?> GetBySubscriptionAndPeriodAsync(Guid subscriptionId, DateTime periodStart, DateTime periodEnd, CancellationToken cancellationToken);
    Task<IReadOnlyList<SubscriptionPayment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken);
    Task AddAsync(SubscriptionPayment payment, CancellationToken cancellationToken);
    Task UpdateAsync(SubscriptionPayment payment, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);
}
