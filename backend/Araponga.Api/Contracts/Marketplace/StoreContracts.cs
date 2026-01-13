namespace Araponga.Api.Contracts.Marketplace;

public sealed record StoreContactPayload(
    string? Phone,
    string? Whatsapp,
    string? Email,
    string? Instagram,
    string? Website,
    string? PreferredContactMethod);

public sealed record UpsertStoreRequest(
    Guid TerritoryId,
    string DisplayName,
    string? Description,
    string ContactVisibility,
    StoreContactPayload? Contact);

public sealed record UpdateStoreRequest(
    string? DisplayName,
    string? Description,
    string? ContactVisibility,
    StoreContactPayload? Contact);

public sealed record StorePaymentRequest(bool Enabled);

public sealed record StoreResponse(
    Guid Id,
    Guid TerritoryId,
    Guid OwnerUserId,
    string DisplayName,
    string? Description,
    string Status,
    bool PaymentsEnabled,
    string ContactVisibility,
    StoreContactPayload? Contact,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);
