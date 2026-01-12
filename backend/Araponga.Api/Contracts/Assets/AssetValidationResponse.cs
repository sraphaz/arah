namespace Araponga.Api.Contracts.Assets;

public sealed record AssetValidationResponse(
    Guid AssetId,
    int ValidationsCount,
    int EligibleResidentsCount,
    double ValidationPct);
