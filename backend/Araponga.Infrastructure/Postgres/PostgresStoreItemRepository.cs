using Araponga.Application.Interfaces;
using Araponga.Domain.Marketplace;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresStoreItemRepository : IStoreItemRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresStoreItemRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<StoreItem?> GetByIdAsync(Guid itemId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.StoreItems
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == itemId, cancellationToken);

        return record?.ToDomain();
    }

    public async Task<IReadOnlyList<StoreItem>> ListByIdsAsync(IReadOnlyCollection<Guid> itemIds, CancellationToken cancellationToken)
    {
        if (itemIds.Count == 0)
        {
            return Array.Empty<StoreItem>();
        }

        var records = await _dbContext.StoreItems
            .AsNoTracking()
            .Where(l => itemIds.Contains(l.Id))
            .ToListAsync(cancellationToken);

        return records.Select(record => record.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<StoreItem>> ListByStoreAsync(Guid storeId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.StoreItems
            .AsNoTracking()
            .Where(l => l.StoreId == storeId)
            .ToListAsync(cancellationToken);

        return records.Select(record => record.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<StoreItem>> SearchAsync(
        Guid territoryId,
        ItemType? type,
        string? query,
        string? category,
        string? tags,
        ItemStatus? status,
        CancellationToken cancellationToken)
    {
        var items = _dbContext.StoreItems.AsNoTracking()
            .Where(l => l.TerritoryId == territoryId);

        if (type is not null)
        {
            items = items.Where(l => l.Type == type);
        }

        if (status is not null)
        {
            items = items.Where(l => l.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            items = items.Where(l => l.Category != null && l.Category == category);
        }

        if (!string.IsNullOrWhiteSpace(tags))
        {
            items = items.Where(l => l.Tags != null && EF.Functions.ILike(l.Tags, $"%{tags}%"));
        }

        if (!string.IsNullOrWhiteSpace(query))
        {
            // Usar full-text search se a coluna search_vector existir (criada pela migration)
            // Fallback para ILike se full-text não estiver disponível
            var searchQuery = query.Trim();
            try
            {
                // Tentar usar full-text search com tsvector (mais performático)
                items = items.Where(l =>
                    EF.Functions.ToTsVector("portuguese", 
                        (l.Title ?? "") + " " + (l.Description ?? ""))
                        .Matches(EF.Functions.PlainToTsQuery("portuguese", searchQuery)));
            }
            catch
            {
                // Fallback para ILike se full-text não estiver disponível
                items = items.Where(l =>
                    EF.Functions.ILike(l.Title, $"%{searchQuery}%") ||
                    (l.Description != null && EF.Functions.ILike(l.Description, $"%{searchQuery}%")));
            }
        }

        var records = await items
            .OrderByDescending(l => l.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return records.Select(record => record.ToDomain()).ToList();
    }

    public Task AddAsync(StoreItem item, CancellationToken cancellationToken)
    {
        _dbContext.StoreItems.Add(item.ToRecord());
        return Task.CompletedTask;
    }

    public Task UpdateAsync(StoreItem item, CancellationToken cancellationToken)
    {
        _dbContext.StoreItems.Update(item.ToRecord());
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<StoreItem>> SearchPagedAsync(
        Guid territoryId,
        ItemType? type,
        string? query,
        string? category,
        string? tags,
        ItemStatus? status,
        int skip,
        int take,
        CancellationToken cancellationToken)
    {
        var items = _dbContext.StoreItems.AsNoTracking()
            .Where(l => l.TerritoryId == territoryId);

        if (type is not null)
        {
            items = items.Where(l => l.Type == type);
        }

        if (status is not null)
        {
            items = items.Where(l => l.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            items = items.Where(l => l.Category != null && l.Category == category);
        }

        if (!string.IsNullOrWhiteSpace(tags))
        {
            items = items.Where(l => l.Tags != null && EF.Functions.ILike(l.Tags, $"%{tags}%"));
        }

        if (!string.IsNullOrWhiteSpace(query))
        {
            // Usar full-text search se disponível
            var searchQuery = query.Trim();
            try
            {
                items = items.Where(l =>
                    EF.Functions.ToTsVector("portuguese", 
                        (l.Title ?? "") + " " + (l.Description ?? ""))
                        .Matches(EF.Functions.PlainToTsQuery("portuguese", searchQuery)));
            }
            catch
            {
                // Fallback para ILike
                items = items.Where(l =>
                    EF.Functions.ILike(l.Title, $"%{searchQuery}%") ||
                    (l.Description != null && EF.Functions.ILike(l.Description, $"%{searchQuery}%")));
            }
        }

        var records = await items
            .OrderByDescending(l => l.CreatedAtUtc)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return records.Select(record => record.ToDomain()).ToList();
    }

    public async Task<int> CountSearchAsync(
        Guid territoryId,
        ItemType? type,
        string? query,
        string? category,
        string? tags,
        ItemStatus? status,
        CancellationToken cancellationToken)
    {
        var items = _dbContext.StoreItems.AsNoTracking()
            .Where(l => l.TerritoryId == territoryId);

        if (type is not null)
        {
            items = items.Where(l => l.Type == type);
        }

        if (status is not null)
        {
            items = items.Where(l => l.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            items = items.Where(l => l.Category != null && l.Category == category);
        }

        if (!string.IsNullOrWhiteSpace(tags))
        {
            items = items.Where(l => l.Tags != null && EF.Functions.ILike(l.Tags, $"%{tags}%"));
        }

        if (!string.IsNullOrWhiteSpace(query))
        {
            // Usar full-text search se disponível
            var searchQuery = query.Trim();
            try
            {
                items = items.Where(l =>
                    EF.Functions.ToTsVector("portuguese", 
                        (l.Title ?? "") + " " + (l.Description ?? ""))
                        .Matches(EF.Functions.PlainToTsQuery("portuguese", searchQuery)));
            }
            catch
            {
                // Fallback para ILike
                items = items.Where(l =>
                    EF.Functions.ILike(l.Title, $"%{searchQuery}%") ||
                    (l.Description != null && EF.Functions.ILike(l.Description, $"%{searchQuery}%")));
            }
        }

        const int maxInt32 = int.MaxValue;
        var count = await items.CountAsync(cancellationToken);
        return count > maxInt32 ? maxInt32 : (int)count;
    }
}
