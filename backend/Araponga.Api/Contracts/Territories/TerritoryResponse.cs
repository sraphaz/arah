namespace Araponga.Api.Contracts.Territories;

public sealed record TerritoryResponse(
    Guid Id,
    string Name,
    string? Description,
    string SensitivityLevel,
    string Status,
    DateTime CreatedAtUtc
);
