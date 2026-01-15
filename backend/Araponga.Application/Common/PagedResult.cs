namespace Araponga.Application.Common;

/// <summary>
/// Represents a paginated result set.
/// </summary>
public sealed class PagedResult<T>
{
    public PagedResult(
        IReadOnlyList<T> items,
        int pageNumber,
        int pageSize,
        int totalCount)
    {
        Items = items;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        HasPreviousPage = PageNumber > 1;
        HasNextPage = PageNumber < TotalPages;
    }

    public IReadOnlyList<T> Items { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public int TotalPages { get; }
    public bool HasPreviousPage { get; }
    public bool HasNextPage { get; }
}

/// <summary>
/// Parameters for pagination.
/// </summary>
public sealed class PaginationParameters
{
    public PaginationParameters(int pageNumber = Constants.Pagination.DefaultPageNumber, int pageSize = Constants.Pagination.DefaultPageSize)
    {
        PageNumber = pageNumber < Constants.Pagination.DefaultPageNumber ? Constants.Pagination.DefaultPageNumber : pageNumber;
        PageSize = ValidationHelpers.NormalizePageSize(pageSize);
    }

    public int PageNumber { get; }
    public int PageSize { get; }
    public int Skip => (PageNumber - 1) * PageSize;
    public int Take => PageSize;
}
