namespace Araponga.Application.Common;

/// <summary>
/// Helper para paginação (Clean Code: DRY, funções pequenas).
/// Reduz duplicação do padrão Skip/Take + PagedResult nos serviços.
/// </summary>
public static class PaginationHelper
{
    private const int MaxInt32 = int.MaxValue;

    /// <summary>
    /// Cria um <see cref="PagedResult{T}"/> a partir de uma lista e parâmetros de paginação.
    /// </summary>
    /// <param name="list">Lista já ordenada/filtrada; a ordem é preservada.</param>
    /// <param name="pagination">Parâmetros de paginação (PageNumber, PageSize).</param>
    /// <returns>Resultado paginado com TotalCount limitado a int.MaxValue.</returns>
    public static PagedResult<T> ToPagedResult<T>(this IReadOnlyList<T> list, PaginationParameters pagination)
    {
        var count = list.Count;
        var totalCount = count > MaxInt32 ? MaxInt32 : count;
        var pagedItems = list
            .Skip(pagination.Skip)
            .Take(pagination.Take)
            .ToList();
        return new PagedResult<T>(pagedItems, pagination.PageNumber, pagination.PageSize, totalCount);
    }
}
