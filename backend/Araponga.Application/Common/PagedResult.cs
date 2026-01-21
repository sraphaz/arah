namespace Araponga.Application.Common;

/// <summary>
/// Represents a paginated result set.
/// </summary>
public sealed class PagedResult<T>
{
    private const int MaxInt32 = int.MaxValue;

    public PagedResult(
        IReadOnlyList<T> items,
        int pageNumber,
        int pageSize,
        int totalCount)
    {
        Items = items;
        PageNumber = pageNumber;
        PageSize = pageSize;
        
        // Proteção contra valores que excedem int.MaxValue
        // Limita totalCount ao máximo seguro para int32
        TotalCount = totalCount > MaxInt32 ? MaxInt32 : totalCount;
        
        // Calcula TotalPages com proteção contra overflow
        if (pageSize <= 0)
        {
            TotalPages = 0;
        }
        else if (TotalCount == 0)
        {
            TotalPages = 0;
        }
        else
        {
            var totalPagesDouble = Math.Ceiling(TotalCount / (double)pageSize);
            TotalPages = totalPagesDouble > MaxInt32 ? MaxInt32 : (int)totalPagesDouble;
        }
        
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
