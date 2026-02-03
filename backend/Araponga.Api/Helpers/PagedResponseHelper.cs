using Araponga.Api.Contracts.Common;
using Araponga.Application.Common;

namespace Araponga.Api.Helpers;

/// <summary>
/// Helper para criar PagedResponse com proteção contra overflow de int32.
/// </summary>
public static class PagedResponseHelper
{
    private const int MaxInt32 = int.MaxValue;

    /// <summary>
    /// Cria um PagedResponse a partir de um PagedResult, protegendo valores contra overflow.
    /// </summary>
    public static PagedResponse<TResponse> ToPagedResponse<TModel, TResponse>(
        PagedResult<TModel> pagedResult,
        Func<TModel, TResponse> mapper)
    {
        var items = pagedResult.Items.Select(mapper).ToList();
        var safeTotalCount = pagedResult.TotalCount > MaxInt32 ? MaxInt32 : pagedResult.TotalCount;
        var safeTotalPages = pagedResult.TotalPages > MaxInt32 ? MaxInt32 : pagedResult.TotalPages;

        return new PagedResponse<TResponse>(
            items,
            pagedResult.PageNumber,
            pagedResult.PageSize,
            safeTotalCount,
            safeTotalPages,
            pagedResult.HasPreviousPage,
            pagedResult.HasNextPage);
    }

    /// <summary>
    /// Normaliza um valor inteiro para garantir que não exceda int.MaxValue.
    /// </summary>
    public static int NormalizeInt(int value)
    {
        return value > MaxInt32 ? MaxInt32 : value;
    }
}
