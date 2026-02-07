namespace Arah.Bff.Contracts.Journeys.Common;

public sealed record JourneyPaginationDto(
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage);
