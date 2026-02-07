using Arah.Bff.Contracts.Journeys.Common;

namespace Arah.Bff.Contracts.Journeys.Feed;

public sealed record TerritoryFeedJourneyResponse(
    IReadOnlyList<TerritoryFeedItemJourneyDto> Items,
    JourneyPaginationDto Pagination,
    TerritoryFeedFiltersDto? Filters);

public sealed record TerritoryFeedItemJourneyDto(
    TerritoryFeedPostDto Post,
    TerritoryFeedCountsDto Counts,
    IReadOnlyList<TerritoryFeedMediaDto> Media,
    TerritoryFeedAuthorDto? Author,
    UserInteractionsDto UserInteractions,
    ItemMetadataDto Metadata);

public sealed record TerritoryFeedPostDto(
    Guid Id,
    string Title,
    string Content,
    string Type,
    string Visibility,
    string Status,
    Guid? MapEntityId,
    DateTime CreatedAtUtc,
    IReadOnlyList<string>? Tags);

public sealed record TerritoryFeedCountsDto(int Likes, int Shares, int Comments);

public sealed record TerritoryFeedMediaDto(
    string Url,
    string Type,
    string? ThumbnailUrl);

public sealed record TerritoryFeedAuthorDto(
    Guid Id,
    string DisplayName,
    string? AvatarUrl);

public sealed record UserInteractionsDto(bool Liked, bool Shared, bool Commented);

public sealed record ItemMetadataDto(bool CanEdit, bool CanDelete, bool CanShare, bool CanComment);

public sealed record TerritoryFeedFiltersDto(
    IReadOnlyList<string> AvailableTypes,
    IReadOnlyList<string> AvailableTags,
    IReadOnlyList<string> AvailableVisibilities);
