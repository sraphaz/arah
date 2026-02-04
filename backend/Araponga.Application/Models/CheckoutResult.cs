using Araponga.Modules.Marketplace.Domain;

namespace Araponga.Application.Models;

public sealed record CheckoutBundle(
    Checkout Checkout,
    IReadOnlyList<CheckoutItem> Items);

public sealed record InquiryBundle(
    Guid InquiryId,
    Guid StoreId,
    Guid ItemId,
    Guid? BatchId);

public sealed record CheckoutSummary(
    Guid StoreId,
    int CheckoutItemCount,
    int InquiryCount);

public sealed record CheckoutResult(
    IReadOnlyList<CheckoutBundle> Checkouts,
    IReadOnlyList<InquiryBundle> Inquiries,
    IReadOnlyList<CheckoutSummary> Summaries);
