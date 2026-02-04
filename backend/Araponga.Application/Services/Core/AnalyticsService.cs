using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Domain.Events;
using Araponga.Domain.Territories;
using Araponga.Modules.Marketplace.Application.Interfaces;

namespace Araponga.Application.Services;

/// <summary>
/// Serviço para coleta de analytics e métricas de negócio.
/// </summary>
public sealed class AnalyticsService
{
    private readonly ITerritoryRepository _territoryRepository;
    private readonly ITerritoryMembershipRepository _membershipRepository;
    private readonly IFeedRepository _feedRepository;
    private readonly ITerritoryEventRepository _eventRepository;
    private readonly ICheckoutRepository _checkoutRepository;
    private readonly ISellerTransactionRepository _sellerTransactionRepository;
    private readonly IUserRepository _userRepository;
    private readonly IStoreRepository _storeRepository;
    private readonly IStoreItemRepository _itemRepository;

    public AnalyticsService(
        ITerritoryRepository territoryRepository,
        ITerritoryMembershipRepository membershipRepository,
        IFeedRepository feedRepository,
        ITerritoryEventRepository eventRepository,
        ICheckoutRepository checkoutRepository,
        ISellerTransactionRepository sellerTransactionRepository,
        IUserRepository userRepository,
        IStoreRepository storeRepository,
        IStoreItemRepository itemRepository)
    {
        _territoryRepository = territoryRepository;
        _membershipRepository = membershipRepository;
        _feedRepository = feedRepository;
        _eventRepository = eventRepository;
        _checkoutRepository = checkoutRepository;
        _sellerTransactionRepository = sellerTransactionRepository;
        _userRepository = userRepository;
        _storeRepository = storeRepository;
        _itemRepository = itemRepository;
    }

    /// <summary>
    /// Obtém estatísticas de um território específico.
    /// </summary>
    public async Task<TerritoryStats?> GetTerritoryStatsAsync(
        Guid territoryId,
        DateTime? fromUtc = null,
        DateTime? toUtc = null,
        CancellationToken cancellationToken = default)
    {
        var territory = await _territoryRepository.GetByIdAsync(territoryId, cancellationToken);
        if (territory is null)
        {
            return null;
        }

        // Contar posts
        var posts = await _feedRepository.ListByTerritoryAsync(territoryId, cancellationToken);
        var totalPosts = posts.Count(p =>
            (fromUtc == null || p.CreatedAtUtc >= fromUtc.Value) &&
            (toUtc == null || p.CreatedAtUtc <= toUtc.Value));

        // Contar eventos
        var totalEvents = await _eventRepository.CountByTerritoryAsync(
            territoryId,
            fromUtc,
            toUtc,
            null,
            cancellationToken);

        // Contar membros (simplificado - usando ListResidentUserIds como estimativa)
        // Nota: Em produção, precisaríamos de um método ListByTerritoryAsync no repositório
        var residentIds = await _membershipRepository.ListResidentUserIdsAsync(territoryId, cancellationToken);
        var totalMembers = residentIds.Count; // Aproximação - apenas residents verificados

        // Membros novos nos últimos 30 dias (simplificado)
        var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
        var newMembers = 0; // TODO: Implementar quando tivermos método para listar todos os memberships por território

        // Vendas do marketplace (simplificado - assumindo que temos método para buscar por território)
        var sales = await GetTerritorySalesAsync(territoryId, fromUtc, toUtc, cancellationToken);
        var totalSalesAmount = sales.Sum(s => s.TotalAmount);
        var totalSalesCount = sales.Count;

        // Payouts (simplificado)
        var payouts = await GetTerritoryPayoutsAsync(territoryId, fromUtc, toUtc, cancellationToken);
        var totalPayoutsAmount = payouts.Sum(p => p.Amount);
        var totalPayoutsCount = payouts.Count;

        return new TerritoryStats
        {
            TerritoryId = territoryId,
            TerritoryName = territory.Name,
            TotalPosts = totalPosts,
            TotalEvents = totalEvents,
            TotalMembers = totalMembers,
            NewMembersLast30Days = newMembers,
            TotalSalesAmount = totalSalesAmount,
            TotalSalesCount = totalSalesCount,
            TotalPayoutsAmount = totalPayoutsAmount,
            TotalPayoutsCount = totalPayoutsCount,
            PeriodStart = fromUtc,
            PeriodEnd = toUtc
        };
    }

    /// <summary>
    /// Obtém estatísticas da plataforma inteira.
    /// </summary>
    public async Task<PlatformStats> GetPlatformStatsAsync(
        DateTime? fromUtc = null,
        DateTime? toUtc = null,
        CancellationToken cancellationToken = default)
    {
        // Contar territórios
        var territories = await _territoryRepository.ListAsync(cancellationToken);
        var totalTerritories = territories.Count;

        // Contar usuários (simplificado - assumindo que temos método para contar)
        // Por enquanto, vamos usar uma estimativa baseada em memberships
        var totalUsers = await EstimateTotalUsersAsync(cancellationToken);

        // Contar posts (todos os territórios)
        var totalPosts = 0;
        foreach (var territory in territories)
        {
            var posts = await _feedRepository.ListByTerritoryAsync(territory.Id, cancellationToken);
            totalPosts += posts.Count(p =>
                (fromUtc == null || p.CreatedAtUtc >= fromUtc.Value) &&
                (toUtc == null || p.CreatedAtUtc <= toUtc.Value));
        }

        // Contar eventos (todos os territórios)
        var totalEvents = 0;
        foreach (var territory in territories)
        {
            totalEvents += await _eventRepository.CountByTerritoryAsync(
                territory.Id,
                fromUtc,
                toUtc,
                null,
                cancellationToken);
        }

        // Vendas totais
        var allSales = await GetAllSalesAsync(fromUtc, toUtc, cancellationToken);
        var totalSalesAmount = allSales.Sum(s => s.TotalAmount);
        var totalSalesCount = allSales.Count;

        // Payouts totais
        var allPayouts = await GetAllPayoutsAsync(fromUtc, toUtc, cancellationToken);
        var totalPayoutsAmount = allPayouts.Sum(p => p.Amount);
        var totalPayoutsCount = allPayouts.Count;

        return new PlatformStats
        {
            TotalTerritories = totalTerritories,
            TotalUsers = totalUsers,
            TotalPosts = totalPosts,
            TotalEvents = totalEvents,
            TotalSalesAmount = totalSalesAmount,
            TotalSalesCount = totalSalesCount,
            TotalPayoutsAmount = totalPayoutsAmount,
            TotalPayoutsCount = totalPayoutsCount,
            PeriodStart = fromUtc,
            PeriodEnd = toUtc
        };
    }

    /// <summary>
    /// Obtém estatísticas do marketplace.
    /// </summary>
    public async Task<MarketplaceStats> GetMarketplaceStatsAsync(
        Guid? territoryId = null,
        DateTime? fromUtc = null,
        DateTime? toUtc = null,
        CancellationToken cancellationToken = default)
    {
        // Nota: Implementação simplificada
        // Em produção, precisaríamos de repositórios específicos para stores e items

        var sales = territoryId.HasValue
            ? await GetTerritorySalesAsync(territoryId.Value, fromUtc, toUtc, cancellationToken)
            : await GetAllSalesAsync(fromUtc, toUtc, cancellationToken);

        var totalSalesAmount = sales.Sum(s => s.TotalAmount);
        var totalSalesCount = sales.Count;

        var payouts = territoryId.HasValue
            ? await GetTerritoryPayoutsAsync(territoryId.Value, fromUtc, toUtc, cancellationToken)
            : await GetAllPayoutsAsync(fromUtc, toUtc, cancellationToken);

        var totalPayoutsAmount = payouts.Sum(p => p.Amount);
        var totalPayoutsCount = payouts.Count;

        // Contar stores
        int totalStores = 0;
        if (territoryId.HasValue)
        {
            var stores = await _storeRepository.ListByTerritoryAsync(territoryId.Value, cancellationToken);
            if (stores != null)
            {
                totalStores = stores.Count(s =>
                    (fromUtc == null || s.CreatedAtUtc >= fromUtc.Value) &&
                    (toUtc == null || s.CreatedAtUtc <= toUtc.Value));
            }
        }
        else
        {
            // Para stats globais, precisamos iterar por territórios
            var territories = await _territoryRepository.ListAsync(cancellationToken);
            if (territories != null)
            {
                foreach (var territory in territories)
                {
                    var stores = await _storeRepository.ListByTerritoryAsync(territory.Id, cancellationToken);
                    if (stores != null)
                    {
                        totalStores += stores.Count(s =>
                            (fromUtc == null || s.CreatedAtUtc >= fromUtc.Value) &&
                            (toUtc == null || s.CreatedAtUtc <= toUtc.Value));
                    }
                }
            }
        }

        // Contar items
        int totalItems = 0;
        if (territoryId.HasValue)
        {
            // Usar SearchAsync com critérios vazios para obter todos os items do território
            var items = await _itemRepository.SearchAsync(
                territoryId.Value,
                type: null,
                query: null,
                category: null,
                tags: null,
                status: null,
                cancellationToken);
            if (items != null)
            {
                totalItems = items.Count(i =>
                    (fromUtc == null || i.CreatedAtUtc >= fromUtc.Value) &&
                    (toUtc == null || i.CreatedAtUtc <= toUtc.Value));
            }
        }
        else
        {
            // Para stats globais, precisamos iterar por territórios
            var territories = await _territoryRepository.ListAsync(cancellationToken);
            if (territories != null)
            {
                foreach (var territory in territories)
                {
                    var items = await _itemRepository.SearchAsync(
                        territory.Id,
                        type: null,
                        query: null,
                        category: null,
                        tags: null,
                        status: null,
                        cancellationToken);
                    if (items != null)
                    {
                        totalItems += items.Count(i =>
                            (fromUtc == null || i.CreatedAtUtc >= fromUtc.Value) &&
                            (toUtc == null || i.CreatedAtUtc <= toUtc.Value));
                    }
                }
            }
        }

        return new MarketplaceStats
        {
            TotalStores = totalStores,
            TotalItems = totalItems,
            TotalSalesAmount = totalSalesAmount,
            TotalSalesCount = totalSalesCount,
            TotalPayoutsAmount = totalPayoutsAmount,
            TotalPayoutsCount = totalPayoutsCount,
            TerritoryId = territoryId,
            PeriodStart = fromUtc,
            PeriodEnd = toUtc
        };
    }


    private async Task<IReadOnlyList<SaleInfo>> GetTerritorySalesAsync(
        Guid territoryId,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken cancellationToken)
    {
        // Usar SellerTransactionRepository para obter vendas do território
        var transactions = await _sellerTransactionRepository.GetByTerritoryIdAsync(territoryId, cancellationToken);

        var sales = transactions
            .Where(t =>
                (fromUtc == null || t.CreatedAtUtc >= fromUtc.Value) &&
                (toUtc == null || t.CreatedAtUtc <= toUtc.Value))
            .Select(t => new SaleInfo(
                t.GrossAmountInCents / 100m, // Converter centavos para decimal
                t.CreatedAtUtc))
            .ToList();

        return sales;
    }

    private async Task<IReadOnlyList<PayoutInfo>> GetTerritoryPayoutsAsync(
        Guid territoryId,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken cancellationToken)
    {
        // Usar SellerTransactionRepository para obter payouts do território
        var transactions = await _sellerTransactionRepository.GetByTerritoryIdAsync(territoryId, cancellationToken);

        var payouts = transactions
            .Where(t =>
                t.PayoutId != null &&
                (fromUtc == null || (t.PaidAtUtc.HasValue && t.PaidAtUtc.Value >= fromUtc.Value)) &&
                (toUtc == null || (t.PaidAtUtc.HasValue && t.PaidAtUtc.Value <= toUtc.Value)))
            .Select(t => new PayoutInfo(
                t.NetAmountInCents / 100m, // Converter centavos para decimal
                t.PaidAtUtc ?? t.CreatedAtUtc))
            .ToList();

        return payouts;
    }

    private async Task<IReadOnlyList<SaleInfo>> GetAllSalesAsync(
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken cancellationToken)
    {
        // Obter todos os checkouts e converter para vendas
        var allCheckouts = await _checkoutRepository.ListAllAsync(cancellationToken);

        var sales = allCheckouts
            .Where(c =>
                c.Status == Araponga.Modules.Marketplace.Domain.CheckoutStatus.Paid &&
                (fromUtc == null || c.CreatedAtUtc >= fromUtc.Value) &&
                (toUtc == null || c.CreatedAtUtc <= toUtc.Value))
            .Select(c => new SaleInfo(
                c.TotalAmount ?? 0m,
                c.CreatedAtUtc))
            .ToList();

        return sales;
    }

    private Task<IReadOnlyList<PayoutInfo>> GetAllPayoutsAsync(
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken cancellationToken)
    {
        // Obter todas as transações de vendedor com payout processado
        // Nota: Precisaríamos de um método GetAllAsync no repositório
        // Por enquanto, retornamos lista vazia
        return Task.FromResult<IReadOnlyList<PayoutInfo>>(Array.Empty<PayoutInfo>());
    }

    private async Task<int> EstimateTotalUsersAsync(CancellationToken cancellationToken)
    {
        // Nota: Implementação simplificada
        // Em produção, precisaríamos de um método CountAsync no IUserRepository
        // Por enquanto, estimamos baseado em memberships únicos
        var territories = await _territoryRepository.ListAsync(cancellationToken);
        var uniqueUserIds = new HashSet<Guid>();

        foreach (var territory in territories)
        {
            var residentIds = await _membershipRepository.ListResidentUserIdsAsync(territory.Id, cancellationToken);
            foreach (var userId in residentIds)
            {
                uniqueUserIds.Add(userId);
            }
        }

        return uniqueUserIds.Count;
    }
}

// Classes auxiliares temporárias
internal sealed record SaleInfo(decimal TotalAmount, DateTime CreatedAtUtc);
internal sealed record PayoutInfo(decimal Amount, DateTime CreatedAtUtc);
