using Araponga.Api.Contracts.Common;

namespace Araponga.Api.Contracts.Users;

public sealed record UserActivityHistoryResponse(
    PagedResponse<UserPostActivityResponse> Posts,
    PagedResponse<UserEventActivityResponse> Events,
    PagedResponse<UserPurchaseActivityResponse> Purchases,
    PagedResponse<UserParticipationActivityResponse> Participations);

public sealed record UserPostActivityResponse(
    Guid Id,
    Guid TerritoryId,
    string Title,
    string Type,
    string Status,
    DateTime CreatedAtUtc,
    DateTime? EditedAtUtc,
    int EditCount);

public sealed record UserEventActivityResponse(
    Guid Id,
    Guid TerritoryId,
    string Title,
    DateTime StartsAtUtc,
    string Status,
    DateTime CreatedAtUtc);

public sealed record UserPurchaseActivityResponse(
    Guid Id,
    Guid TerritoryId,
    decimal TotalAmount,
    string Currency,
    string Status,
    DateTime CreatedAtUtc);

public sealed record UserSaleActivityResponse(
    Guid Id,
    Guid TerritoryId,
    decimal TotalAmount,
    string Currency,
    string Status,
    DateTime CreatedAtUtc);

public sealed record UserParticipationActivityResponse(
    Guid EventId,
    Guid TerritoryId,
    string EventTitle,
    DateTime StartsAtUtc,
    string Status,
    DateTime CreatedAtUtc);
