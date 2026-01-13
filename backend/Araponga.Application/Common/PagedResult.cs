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
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 100;

    public PaginationParameters(int pageNumber = 1, int pageSize = DefaultPageSize)
    {
        PageNumber = pageNumber < 1 ? 1 : pageNumber;
        PageSize = pageSize < 1 ? DefaultPageSize : (pageSize > MaxPageSize ? MaxPageSize : pageSize);
    }

    public int PageNumber { get; }
    public int PageSize { get; }
    public int Skip => (PageNumber - 1) * PageSize;
    public int Take => PageSize;
}
