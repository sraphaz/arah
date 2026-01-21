namespace Araponga.Application.Models;

public sealed record UserSaleActivity(
    Guid Id,
    Guid TerritoryId,
    decimal TotalAmount,
    string Currency,
    string Status,
    DateTime CreatedAtUtc);
