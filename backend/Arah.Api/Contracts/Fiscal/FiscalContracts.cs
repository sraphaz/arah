namespace Arah.Api.Contracts.Fiscal;

public sealed record FiscalPackResponse(
    string Id,
    string CountryCode,
    string DisplayName,
    IReadOnlyList<string> Capabilities);

public sealed record TerritoryFiscalPackBindingResponse(
    Guid TerritoryId,
    string? PackId,
    string Status,
    Guid? ActivatedByUserId,
    DateTimeOffset? ActivatedAtUtc,
    string? MunicipalityIbge,
    bool IsLegacyDefault);

public sealed record UpsertTerritoryFiscalPackBindingRequest(
    string PackId,
    string Status,
    string? MunicipalityIbge);

public sealed record TerritoryPaymentMethodsResponse(
    Guid TerritoryId,
    IReadOnlyList<string> Methods,
    string? PspProvider,
    DateTimeOffset? UpdatedAtUtc,
    bool IsDefaultEmpty);

public sealed record UpsertTerritoryPaymentMethodsRequest(
    IReadOnlyList<string> Methods,
    string? PspProvider);
