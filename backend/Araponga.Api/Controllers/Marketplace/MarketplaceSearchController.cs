using Araponga.Api.Contracts.Common;
using Araponga.Api.Contracts.Marketplace;
using Araponga.Api.Security;
using Araponga.Application.Common;
using Araponga.Application.Models;
using Araponga.Application.Services;
using Araponga.Domain.Marketplace;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1")]
[Produces("application/json")]
[Tags("Marketplace Search")]
public sealed class MarketplaceSearchController : ControllerBase
{
    private readonly MarketplaceSearchService _searchService;
    private readonly CurrentUserAccessor _currentUserAccessor;

    public MarketplaceSearchController(
        MarketplaceSearchService searchService,
        CurrentUserAccessor currentUserAccessor)
    {
        _searchService = searchService;
        _currentUserAccessor = currentUserAccessor;
    }

    /// <summary>
    /// Busca geral no marketplace (lojas e itens).
    /// </summary>
    [HttpGet("marketplace/search")]
    [EnableRateLimiting("feed")]
    [ProducesResponseType(typeof(SearchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<SearchResponse>> SearchAll(
        [FromQuery] Guid territoryId,
        [FromQuery] string? query,
        [FromQuery] string? category,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] string? currency,
        [FromQuery] double? latitude,
        [FromQuery] double? longitude,
        [FromQuery] double? radiusKm,
        [FromQuery] double? minRating,
        [FromQuery] string? sortOrder,
        CancellationToken cancellationToken,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid)
        {
            return Unauthorized();
        }

        var filters = new SearchFilters
        {
            Query = query,
            Category = category,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            Currency = currency,
            Latitude = latitude,
            Longitude = longitude,
            RadiusKm = radiusKm,
            MinRating = minRating,
            SortOrder = ParseSortOrder(sortOrder)
        };

        var pagination = new PaginationParameters(pageNumber, pageSize);
        var result = await _searchService.SearchAllAsync(territoryId, filters, pagination, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error ?? "Search failed." });
        }

        var combined = result.Value!;
        const int maxInt32 = int.MaxValue;
        var safeTotalCount = combined.TotalCount > maxInt32 ? maxInt32 : combined.TotalCount;
        var response = new SearchResponse(
            combined.Stores.Select(ToStoreResponse).ToList(),
            combined.Items.Select(ToItemResponse).ToList(),
            safeTotalCount);

        return Ok(response);
    }

    /// <summary>
    /// Busca lojas no marketplace.
    /// </summary>
    [HttpGet("stores/search")]
    [EnableRateLimiting("feed")]
    [ProducesResponseType(typeof(PagedResponse<StoreSearchResultResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResponse<StoreSearchResultResponse>>> SearchStores(
        [FromQuery] Guid territoryId,
        [FromQuery] string? query,
        [FromQuery] double? minRating,
        [FromQuery] string? sortOrder,
        CancellationToken cancellationToken,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid)
        {
            return Unauthorized();
        }

        var filters = new SearchFilters
        {
            Query = query,
            MinRating = minRating,
            SortOrder = ParseSortOrder(sortOrder)
        };

        var pagination = new PaginationParameters(pageNumber, pageSize);
        var result = await _searchService.SearchStoresAsync(territoryId, filters, pagination, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error ?? "Search failed." });
        }

        var pagedResult = result.Value!;
        var items = pagedResult.Items.Select(ToStoreResponse).ToList();
        const int maxInt32 = int.MaxValue;
        var safeTotalCount = pagedResult.TotalCount > maxInt32 ? maxInt32 : pagedResult.TotalCount;
        var safeTotalPages = pagedResult.TotalPages > maxInt32 ? maxInt32 : pagedResult.TotalPages;
        var response = new PagedResponse<StoreSearchResultResponse>(
            items,
            pagedResult.PageNumber,
            pagedResult.PageSize,
            safeTotalCount,
            safeTotalPages,
            pagedResult.HasPreviousPage,
            pagedResult.HasNextPage);

        return Ok(response);
    }

    /// <summary>
    /// Busca itens no marketplace.
    /// </summary>
    [HttpGet("items/search")]
    [EnableRateLimiting("feed")]
    [ProducesResponseType(typeof(PagedResponse<ItemSearchResultResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResponse<ItemSearchResultResponse>>> SearchItems(
        [FromQuery] Guid territoryId,
        [FromQuery] string? query,
        [FromQuery] string? category,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] string? currency,
        [FromQuery] double? latitude,
        [FromQuery] double? longitude,
        [FromQuery] double? radiusKm,
        [FromQuery] double? minRating,
        [FromQuery] string? sortOrder,
        CancellationToken cancellationToken,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid)
        {
            return Unauthorized();
        }

        var filters = new SearchFilters
        {
            Query = query,
            Category = category,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            Currency = currency,
            Latitude = latitude,
            Longitude = longitude,
            RadiusKm = radiusKm,
            MinRating = minRating,
            SortOrder = ParseSortOrder(sortOrder)
        };

        var pagination = new PaginationParameters(pageNumber, pageSize);
        var result = await _searchService.SearchItemsAsync(territoryId, filters, pagination, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error ?? "Search failed." });
        }

        var pagedResult = result.Value!;
        var items = pagedResult.Items.Select(ToItemResponse).ToList();
        const int maxInt32 = int.MaxValue;
        var safeTotalCount = pagedResult.TotalCount > maxInt32 ? maxInt32 : pagedResult.TotalCount;
        var safeTotalPages = pagedResult.TotalPages > maxInt32 ? maxInt32 : pagedResult.TotalPages;
        var response = new PagedResponse<ItemSearchResultResponse>(
            items,
            pagedResult.PageNumber,
            pagedResult.PageSize,
            safeTotalCount,
            safeTotalPages,
            pagedResult.HasPreviousPage,
            pagedResult.HasNextPage);

        return Ok(response);
    }

    private static SearchSortOrder ParseSortOrder(string? sortOrder)
    {
        if (string.IsNullOrWhiteSpace(sortOrder))
        {
            return SearchSortOrder.Relevance;
        }

        return Enum.TryParse<SearchSortOrder>(sortOrder, true, out var parsed)
            ? parsed
            : SearchSortOrder.Relevance;
    }

    private static StoreSearchResultResponse ToStoreResponse(StoreSearchResult result)
    {
        var store = result.Store;
        return new StoreSearchResultResponse(
            store.Id,
            store.TerritoryId,
            store.OwnerUserId,
            store.DisplayName,
            store.Description,
            store.Status.ToString().ToUpperInvariant(),
            store.PaymentsEnabled,
            result.AverageRating,
            store.CreatedAtUtc);
    }

    private static ItemSearchResultResponse ToItemResponse(ItemSearchResult result)
    {
        var item = result.Item;
        return new ItemSearchResultResponse(
            item.Id,
            item.TerritoryId,
            item.StoreId,
            item.Type.ToString().ToUpperInvariant(),
            item.Title,
            item.Description,
            item.Category,
            item.Tags,
            item.PricingType.ToString().ToUpperInvariant(),
            item.PriceAmount,
            item.Currency,
            item.Unit,
            item.Latitude,
            item.Longitude,
            item.Status.ToString().ToUpperInvariant(),
            result.AverageRating,
            item.CreatedAtUtc);
    }
}
