using Araponga.Domain.Territories;

namespace Araponga.Application.Models;

public sealed record TerritoryCreationResult(bool Success, string? Error, Territory? Territory);
