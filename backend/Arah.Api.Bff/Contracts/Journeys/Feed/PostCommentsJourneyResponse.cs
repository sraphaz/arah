using Arah.Bff.Contracts.Journeys.Common;

namespace Arah.Bff.Contracts.Journeys.Feed;

public sealed record PostCommentsJourneyResponse(
    IReadOnlyList<PostCommentItemDto> Items,
    JourneyPaginationDto Pagination);

public sealed record PostCommentItemDto(
    Guid Id,
    string Content,
    DateTime CreatedAtUtc,
    PostCommentAuthorDto Author);

public sealed record PostCommentAuthorDto(
    Guid Id,
    string DisplayName,
    string? AvatarUrl);
