using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Domain.Geo;
using Araponga.Domain.Marketplace;
using static Araponga.Application.Common.Constants;

namespace Araponga.Application.Services;

/// <summary>
/// Service responsible for advanced marketplace search with full-text search, filters, and sorting.
/// </summary>
public sealed class MarketplaceSearchService
{
    private readonly IStoreRepository _storeRepository;
    private readonly IStoreItemRepository _itemRepository;
    private readonly IStoreRatingRepository _storeRatingRepository;
    private readonly IStoreItemRatingRepository _itemRatingRepository;

    public MarketplaceSearchService(
        IStoreRepository storeRepository,
        IStoreItemRepository itemRepository,
        IStoreRatingRepository storeRatingRepository,
        IStoreItemRatingRepository itemRatingRepository)
    {
        _storeRepository = storeRepository;
        _itemRepository = itemRepository;
        _storeRatingRepository = storeRatingRepository;
        _itemRatingRepository = itemRatingRepository;
    }

    /// <summary>
    /// Searches stores with advanced filters and sorting.
    /// </summary>
    public async Task<Result<PagedResult<StoreSearchResult>>> SearchStoresAsync(
        Guid territoryId,
        SearchFilters filters,
        PaginationParameters pagination,
        CancellationToken cancellationToken)
    {
        // Por enquanto, usar busca simples baseada em nome
        // TODO: Implementar full-text search com PostgreSQL quando necessário
        
        var allStores = await _storeRepository.ListByTerritoryAsync(territoryId, cancellationToken);

        // Filtrar por status ativo
        var stores = allStores
            .Where(s => s.Status == StoreStatus.Active)
            .AsEnumerable();

        // Aplicar filtro de query (busca por nome)
        if (!string.IsNullOrWhiteSpace(filters.Query))
        {
            var queryLower = filters.Query.ToLowerInvariant();
            stores = stores.Where(s =>
                s.DisplayName.ToLowerInvariant().Contains(queryLower) ||
                (s.Description != null && s.Description.ToLowerInvariant().Contains(queryLower)));
        }

        // Aplicar filtro de rating mínimo
        if (filters.MinRating.HasValue && filters.MinRating.Value > 0)
        {
            var storeIds = stores.Select(s => s.Id).ToList();
            var storesWithRatings = new List<Store>();
            
            foreach (var store in stores)
            {
                var averageRating = await _storeRatingRepository.GetAverageRatingAsync(store.Id, cancellationToken);
                if (averageRating >= filters.MinRating.Value)
                {
                    storesWithRatings.Add(store);
                }
            }
            
            stores = storesWithRatings;
        }

        // Aplicar ordenação
        stores = ApplyStoreSorting(stores, filters.SortOrder);

        const int maxInt32 = int.MaxValue;
        var count = stores.Count();
        var totalCount = count > maxInt32 ? maxInt32 : count;
        var pagedStores = stores
            .Skip(pagination.Skip)
            .Take(pagination.Take)
            .ToList();

        // Construir resultados com ratings
        var results = new List<StoreSearchResult>();
        foreach (var store in pagedStores)
        {
            var averageRating = await _storeRatingRepository.GetAverageRatingAsync(store.Id, cancellationToken);
            results.Add(new StoreSearchResult(store, averageRating));
        }

        var pagedResult = new PagedResult<StoreSearchResult>(
            results,
            pagination.PageNumber,
            pagination.PageSize,
            totalCount);

        return Result<PagedResult<StoreSearchResult>>.Success(pagedResult);
    }

    /// <summary>
    /// Searches items with advanced filters and sorting.
    /// </summary>
    public async Task<Result<PagedResult<ItemSearchResult>>> SearchItemsAsync(
        Guid territoryId,
        SearchFilters filters,
        PaginationParameters pagination,
        CancellationToken cancellationToken)
    {
        // Usar busca existente do repositório
        var totalCount = await _itemRepository.CountSearchAsync(
            territoryId,
            null, // type - pode ser adicionado aos filtros
            filters.Query,
            filters.Category,
            null, // tags - pode ser adicionado aos filtros
            ItemStatus.Active,
            cancellationToken);

        var items = await _itemRepository.SearchPagedAsync(
            territoryId,
            null, // type
            filters.Query,
            filters.Category,
            null, // tags
            ItemStatus.Active,
            pagination.Skip,
            pagination.Take,
            cancellationToken);

        // Aplicar filtros adicionais
        var filteredItems = items.AsEnumerable();

        // Filtro de preço
        if (filters.MinPrice.HasValue || filters.MaxPrice.HasValue)
        {
            filteredItems = filteredItems.Where(item =>
            {
                if (item.PriceAmount is null)
                {
                    return false;
                }

                if (filters.Currency is not null && item.Currency != filters.Currency)
                {
                    return false;
                }

                if (filters.MinPrice.HasValue && item.PriceAmount < filters.MinPrice.Value)
                {
                    return false;
                }

                if (filters.MaxPrice.HasValue && item.PriceAmount > filters.MaxPrice.Value)
                {
                    return false;
                }

                return true;
            });
        }

        // Filtro de localização (raio)
        if (filters.Latitude.HasValue && filters.Longitude.HasValue && filters.RadiusKm.HasValue)
        {
            if (GeoCoordinate.IsValid(filters.Latitude.Value, filters.Longitude.Value))
            {
                filteredItems = filteredItems.Where(item =>
                {
                    if (item.Latitude is null || item.Longitude is null)
                    {
                        return false;
                    }

                    var distance = CalculateDistance(
                        filters.Latitude.Value,
                        filters.Longitude.Value,
                        item.Latitude.Value,
                        item.Longitude.Value);

                    return distance <= filters.RadiusKm.Value;
                });
            }
        }

        // Aplicar ordenação
        filteredItems = ApplyItemSorting(filteredItems, filters.SortOrder);

        var itemsList = filteredItems.ToList();

        // Construir resultados com ratings
        var results = new List<ItemSearchResult>();
        foreach (var item in itemsList)
        {
            var averageRating = await _itemRatingRepository.GetAverageRatingAsync(item.Id, cancellationToken);
            
            // Aplicar filtro de rating mínimo
            if (filters.MinRating.HasValue && averageRating < filters.MinRating.Value)
            {
                continue;
            }

            results.Add(new ItemSearchResult(item, averageRating));
        }

        // Recalcular total após filtros
        const int maxInt32 = int.MaxValue;
        var count = results.Count;
        var finalTotalCount = count > maxInt32 ? maxInt32 : count;

        var pagedResult = new PagedResult<ItemSearchResult>(
            results,
            pagination.PageNumber,
            pagination.PageSize,
            finalTotalCount);

        return Result<PagedResult<ItemSearchResult>>.Success(pagedResult);
    }

    /// <summary>
    /// Searches both stores and items.
    /// </summary>
    public async Task<Result<CombinedSearchResult>> SearchAllAsync(
        Guid territoryId,
        SearchFilters filters,
        PaginationParameters pagination,
        CancellationToken cancellationToken)
    {
        var storesTask = SearchStoresAsync(territoryId, filters, pagination, cancellationToken);
        var itemsTask = SearchItemsAsync(territoryId, filters, pagination, cancellationToken);

        await Task.WhenAll(storesTask, itemsTask);

        var storesResult = await storesTask;
        var itemsResult = await itemsTask;

        if (!storesResult.IsSuccess || !itemsResult.IsSuccess)
        {
            return Result<CombinedSearchResult>.Failure(
                storesResult.Error ?? itemsResult.Error ?? "Search failed.");
        }

        const int maxInt32 = int.MaxValue;
        var storesTotal = storesResult.Value?.TotalCount ?? 0;
        var itemsTotal = itemsResult.Value?.TotalCount ?? 0;
        var combinedTotal = storesTotal + itemsTotal;
        var safeTotalCount = combinedTotal > maxInt32 ? maxInt32 : combinedTotal;
        
        var combined = new CombinedSearchResult(
            storesResult.Value?.Items ?? Array.Empty<StoreSearchResult>(),
            itemsResult.Value?.Items ?? Array.Empty<ItemSearchResult>(),
            safeTotalCount);

        return Result<CombinedSearchResult>.Success(combined);
    }

    private static IEnumerable<Store> ApplyStoreSorting(IEnumerable<Store> stores, SearchSortOrder sortOrder)
    {
        return sortOrder switch
        {
            SearchSortOrder.DateDescending => stores.OrderByDescending(s => s.CreatedAtUtc),
            _ => stores.OrderBy(s => s.DisplayName) // Default: ordenar por nome
        };
    }

    private static IEnumerable<StoreItem> ApplyItemSorting(IEnumerable<StoreItem> items, SearchSortOrder sortOrder)
    {
        return sortOrder switch
        {
            SearchSortOrder.PriceAscending => items.OrderBy(i => i.PriceAmount ?? decimal.MaxValue),
            SearchSortOrder.PriceDescending => items.OrderByDescending(i => i.PriceAmount ?? 0),
            SearchSortOrder.DateDescending => items.OrderByDescending(i => i.CreatedAtUtc),
            _ => items.OrderByDescending(i => i.CreatedAtUtc) // Default: mais recente
        };
    }

    private static double CalculateDistance(
        double latitude1,
        double longitude1,
        double latitude2,
        double longitude2)
    {
        var lat1 = DegreesToRadians(latitude1);
        var lat2 = DegreesToRadians(latitude2);
        var deltaLat = DegreesToRadians(latitude2 - latitude1);
        var deltaLon = DegreesToRadians(longitude2 - longitude1);

        var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                Math.Cos(lat1) * Math.Cos(lat2) *
                Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return Constants.Geography.EarthRadiusKm * c;
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }
}
