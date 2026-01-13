namespace Araponga.Api.Contracts.Marketplace;

public sealed record CreateInquiryRequest(string? Message);

public sealed record StoreContactResponse(
    string ContactVisibility,
    string? Phone,
    string? Whatsapp,
    string? Email,
    string? Instagram,
    string? Website,
    string? PreferredContactMethod);

public sealed record InquiryResponse(
    Guid Id,
    Guid TerritoryId,
    Guid ListingId,
    Guid StoreId,
    Guid FromUserId,
    string? Message,
    string Status,
    Guid? BatchId,
    DateTime CreatedAtUtc,
    StoreContactResponse? StoreContact);
