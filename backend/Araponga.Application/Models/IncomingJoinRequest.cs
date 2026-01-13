namespace Araponga.Application.Models;

public sealed record IncomingJoinRequest(
    Guid Id,
    Guid TerritoryId,
    Guid RequesterUserId,
    string RequesterDisplayName,
    string? Message,
    DateTime CreatedAtUtc);
