namespace Araponga.Api.Contracts.Common;

/// <summary>
/// Response contract for paginated results.
/// </summary>
public sealed record PagedResponse<T>(
    IReadOnlyList<T> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage);
