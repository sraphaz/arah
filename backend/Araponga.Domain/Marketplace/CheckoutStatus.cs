namespace Araponga.Domain.Marketplace;

public enum CheckoutStatus
{
    Created = 1,
    AwaitingPayment = 2,
    Paid = 3,
    Canceled = 4
}
