namespace Araponga.Api.Contracts.Marketplace;

public sealed record AddCartItemRequest(
    Guid TerritoryId,
    Guid ListingId,
    int Quantity,
    string? Notes);

public sealed record UpdateCartItemRequest(
    int Quantity,
    string? Notes);

public sealed record CartItemResponse(
    Guid Id,
    Guid ListingId,
    int Quantity,
    string? Notes,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc,
    ListingResponse Listing,
    StoreResponse Store,
    bool IsPurchasable);

public sealed record CartResponse(
    Guid CartId,
    Guid TerritoryId,
    Guid UserId,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc,
    IReadOnlyList<CartItemResponse> Items);

public sealed record CartCheckoutRequest(
    Guid TerritoryId,
    string? Message);

public sealed record CheckoutItemResponse(
    Guid Id,
    Guid ListingId,
    string ListingType,
    string TitleSnapshot,
    int Quantity,
    decimal? UnitPrice,
    decimal? LineSubtotal,
    decimal? PlatformFeeLine,
    decimal? LineTotal,
    DateTime CreatedAtUtc);

public sealed record CheckoutResponse(
    Guid Id,
    Guid TerritoryId,
    Guid BuyerUserId,
    Guid StoreId,
    string Status,
    string Currency,
    decimal? ItemsSubtotalAmount,
    decimal? PlatformFeeAmount,
    decimal? TotalAmount,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc,
    IReadOnlyList<CheckoutItemResponse> Items);

public sealed record InquiryBundleResponse(
    Guid Id,
    Guid StoreId,
    Guid ListingId,
    Guid? BatchId);

public sealed record CheckoutSummaryResponse(
    Guid StoreId,
    int CheckoutItemCount,
    int InquiryCount);

public sealed record CartCheckoutResponse(
    IReadOnlyList<CheckoutResponse> Checkouts,
    IReadOnlyList<InquiryBundleResponse> Inquiries,
    IReadOnlyList<CheckoutSummaryResponse> Summaries);
