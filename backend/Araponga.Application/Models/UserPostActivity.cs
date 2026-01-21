namespace Araponga.Application.Models;

public sealed record UserPostActivity(
    Guid Id,
    Guid TerritoryId,
    string Title,
    string Type,
    string Status,
    DateTime CreatedAtUtc,
    DateTime? EditedAtUtc,
    int EditCount);
