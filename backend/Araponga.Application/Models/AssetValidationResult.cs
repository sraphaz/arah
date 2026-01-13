namespace Araponga.Application.Models;

public sealed record AssetValidationResult(
    AssetDetails Asset,
    bool Created);
