namespace Araponga.Api.Contracts.Territories;

public sealed record CreateTerritoryRequest(
    string Name,
    string? Description,
    string SensitivityLevel,
    bool IsPilot
);
