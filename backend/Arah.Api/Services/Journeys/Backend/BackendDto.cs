namespace Arah.Api.Services.Journeys.Backend;

/// <summary>
/// DTOs do contrato do backend para o BFF. Não referenciam Domain/Application;
/// permitem trocar a implementação (in-process ou HTTP) sem alterar o BFF.
/// </summary>

public sealed record BackendPagedResult<T>(
    IReadOnlyList<T> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage);

public sealed record BackendFeedPost(
    Guid Id,
    Guid TerritoryId,
    Guid AuthorUserId,
    string Title,
    string Content,
    string Type,
    string Visibility,
    string Status,
    Guid? MapEntityId,
    string? ReferenceType,
    Guid? ReferenceId,
    DateTime CreatedAtUtc,
    IReadOnlyList<string>? Tags);

public sealed record BackendPostCounts(int LikeCount, int ShareCount, int CommentCount = 0);

public sealed record BackendPostComment(
    Guid Id,
    Guid PostId,
    Guid UserId,
    string Content,
    DateTime CreatedAtUtc);

public sealed record BackendEventSummary(
    Guid EventId,
    string Title,
    string? Description,
    DateTime StartsAtUtc,
    DateTime? EndsAtUtc,
    double? Latitude,
    double? Longitude,
    string? LocationLabel,
    string Status,
    string CreatedByMembership,
    string? CreatedByDisplayName,
    int InterestedCount,
    int ConfirmedCount);

public sealed record BackendUserInfo(Guid Id, string DisplayName, string? AvatarUrl);

public sealed record BackendCreatePostResult(bool Success, BackendFeedPost? Post, string? Error);

public sealed record BackendTerritoryInfo(Guid Id, string Name, string? Description);

public sealed record BackendMembershipInfo(string Role);

public sealed record BackendEventDetail(
    Guid Id,
    Guid TerritoryId,
    string Title,
    string? Description,
    DateTime StartsAtUtc,
    DateTime? EndsAtUtc,
    double? Latitude,
    double? Longitude,
    string? LocationLabel,
    string Status,
    string CreatedByMembership,
    string? CreatedByDisplayName,
    int InterestedCount,
    int ConfirmedCount);

public sealed record BackendEventParticipationStatus(Guid EventId, string Status);

public sealed record BackendCreateEventResult(bool Success, BackendEventDetail? Event, string? Error);
