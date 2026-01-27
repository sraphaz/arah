using Araponga.Domain.Subscriptions;

namespace Araponga.Infrastructure.Postgres.Entities;

public sealed class SubscriptionPaymentRecord
{
    public Guid Id { get; set; }
    public Guid SubscriptionId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "BRL";
    public int Status { get; set; } // SubscriptionPaymentStatus enum
    public DateTime PaymentDate { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public string? StripeInvoiceId { get; set; }
    public string? StripePaymentIntentId { get; set; }
    public string? FailureReason { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
