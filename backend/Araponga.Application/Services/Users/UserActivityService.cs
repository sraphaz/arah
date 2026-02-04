using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Domain.Events;
using Araponga.Domain.Feed;
using Araponga.Modules.Marketplace.Application.Interfaces;

namespace Araponga.Application.Services;

/// <summary>
/// Service responsible for retrieving user activity history.
/// </summary>
public sealed class UserActivityService
{
    private readonly IFeedRepository _feedRepository;
    private readonly ITerritoryEventRepository _eventRepository;
    private readonly ICheckoutRepository _checkoutRepository;
    private readonly IEventParticipationRepository _participationRepository;
    private readonly IStoreRepository _storeRepository;

    public UserActivityService(
        IFeedRepository feedRepository,
        ITerritoryEventRepository eventRepository,
        ICheckoutRepository checkoutRepository,
        IEventParticipationRepository participationRepository,
        IStoreRepository storeRepository)
    {
        _feedRepository = feedRepository;
        _eventRepository = eventRepository;
        _checkoutRepository = checkoutRepository;
        _participationRepository = participationRepository;
        _storeRepository = storeRepository;
    }

    /// <summary>
    /// Gets posts created by the user.
    /// </summary>
    public async Task<PagedResult<UserPostActivity>> GetUserPostsAsync(
        Guid userId,
        PaginationParameters pagination,
        CancellationToken cancellationToken)
    {
        var totalCount = await _feedRepository.CountByAuthorAsync(userId, cancellationToken);
        var posts = await _feedRepository.ListByAuthorPagedAsync(
            userId,
            pagination.Skip,
            pagination.Take,
            cancellationToken);

        var activities = posts.Select(p => new UserPostActivity(
            p.Id,
            p.TerritoryId,
            p.Title,
            p.Type.ToString(),
            p.Status.ToString(),
            p.CreatedAtUtc,
            p.EditedAtUtc,
            p.EditCount)).ToList();

        return new PagedResult<UserPostActivity>(
            activities,
            pagination.PageNumber,
            pagination.PageSize,
            totalCount);
    }

    /// <summary>
    /// Gets events created by the user.
    /// </summary>
    public async Task<PagedResult<UserEventActivity>> GetUserEventsAsync(
        Guid userId,
        PaginationParameters pagination,
        CancellationToken cancellationToken)
    {
        var totalCount = await _eventRepository.CountByAuthorAsync(userId, cancellationToken);
        var events = await _eventRepository.ListByAuthorPagedAsync(
            userId,
            pagination.Skip,
            pagination.Take,
            cancellationToken);

        var activities = events.Select(e => new UserEventActivity(
            e.Id,
            e.TerritoryId,
            e.Title,
            e.StartsAtUtc,
            e.Status.ToString(),
            e.CreatedAtUtc)).ToList();

        return new PagedResult<UserEventActivity>(
            activities,
            pagination.PageNumber,
            pagination.PageSize,
            totalCount);
    }

    /// <summary>
    /// Gets purchases made by the user.
    /// </summary>
    public async Task<PagedResult<UserPurchaseActivity>> GetUserPurchasesAsync(
        Guid userId,
        PaginationParameters pagination,
        CancellationToken cancellationToken)
    {
        var checkouts = await _checkoutRepository.ListByUserAsync(userId, cancellationToken);

        var purchases = checkouts
            .Where(c => c.Status == Araponga.Modules.Marketplace.Domain.CheckoutStatus.Paid)
            .OrderByDescending(c => c.CreatedAtUtc)
            .Skip(pagination.Skip)
            .Take(pagination.Take)
            .Select(c => new UserPurchaseActivity(
                c.Id,
                c.TerritoryId,
                c.TotalAmount ?? 0,
                c.Currency,
                c.Status.ToString(),
                c.CreatedAtUtc))
            .ToList();

        const int maxInt32 = int.MaxValue;
        var count = checkouts.Count(c => c.Status == Araponga.Modules.Marketplace.Domain.CheckoutStatus.Paid);
        var totalCount = count > maxInt32 ? maxInt32 : count;

        return new PagedResult<UserPurchaseActivity>(
            purchases,
            pagination.PageNumber,
            pagination.PageSize,
            totalCount);
    }

    /// <summary>
    /// Gets sales made by the user (as store owner).
    /// </summary>
    public async Task<PagedResult<UserSaleActivity>> GetUserSalesAsync(
        Guid userId,
        PaginationParameters pagination,
        CancellationToken cancellationToken)
    {
        // Buscar lojas do usuário
        var userStores = await _storeRepository.ListByOwnerAsync(userId, cancellationToken);
        var storeIds = userStores.Select(s => s.Id).ToHashSet();

        if (storeIds.Count == 0)
        {
            return new PagedResult<UserSaleActivity>(
                Array.Empty<UserSaleActivity>(),
                pagination.PageNumber,
                pagination.PageSize,
                0);
        }

        // Buscar checkouts das lojas do usuário
        var allCheckouts = await _checkoutRepository.ListAllAsync(cancellationToken);

        var sales = allCheckouts
            .Where(c => storeIds.Contains(c.StoreId) && c.Status == Araponga.Modules.Marketplace.Domain.CheckoutStatus.Paid)
            .OrderByDescending(c => c.CreatedAtUtc)
            .Skip(pagination.Skip)
            .Take(pagination.Take)
            .Select(c => new UserSaleActivity(
                c.Id,
                c.TerritoryId,
                c.TotalAmount ?? 0,
                c.Currency,
                c.Status.ToString(),
                c.CreatedAtUtc))
            .ToList();

        const int maxInt32 = int.MaxValue;
        var count = allCheckouts.Count(c => storeIds.Contains(c.StoreId) && c.Status == Araponga.Modules.Marketplace.Domain.CheckoutStatus.Paid);
        var totalCount = count > maxInt32 ? maxInt32 : count;

        return new PagedResult<UserSaleActivity>(
            sales,
            pagination.PageNumber,
            pagination.PageSize,
            totalCount);
    }

    /// <summary>
    /// Gets event participations by the user.
    /// </summary>
    public async Task<PagedResult<UserParticipationActivity>> GetUserParticipationsAsync(
        Guid userId,
        PaginationParameters pagination,
        CancellationToken cancellationToken)
    {
        var participations = await _participationRepository.GetByUserIdAsync(userId, cancellationToken);

        var totalCount = participations.Count;
        var pagedParticipations = participations
            .OrderByDescending(p => p.CreatedAtUtc)
            .Skip(pagination.Skip)
            .Take(pagination.Take)
            .ToList();

        // Buscar eventos para obter título e data (otimizado: buscar todos de uma vez)
        var eventIds = pagedParticipations.Select(p => p.EventId).Distinct().ToList();
        var events = await _eventRepository.ListByIdsAsync(eventIds, cancellationToken);
        var eventDict = events.ToDictionary(e => e.Id);

        var activities = pagedParticipations
            .Select(p =>
            {
                var evt = eventDict.GetValueOrDefault(p.EventId);
                return new UserParticipationActivity(
                    p.EventId,
                    evt?.TerritoryId ?? Guid.Empty,
                    evt?.Title ?? "Evento não encontrado",
                    evt?.StartsAtUtc ?? DateTime.UtcNow,
                    p.Status.ToString(),
                    p.CreatedAtUtc);
            })
            .ToList();

        return new PagedResult<UserParticipationActivity>(
            activities,
            pagination.PageNumber,
            pagination.PageSize,
            totalCount);
    }

    /// <summary>
    /// Gets complete activity history for the user.
    /// </summary>
    public async Task<UserActivityHistory> GetUserActivityHistoryAsync(
        Guid userId,
        PaginationParameters pagination,
        CancellationToken cancellationToken)
    {
        var postsTask = GetUserPostsAsync(userId, pagination, cancellationToken);
        var eventsTask = GetUserEventsAsync(userId, pagination, cancellationToken);
        var purchasesTask = GetUserPurchasesAsync(userId, pagination, cancellationToken);
        var participationsTask = GetUserParticipationsAsync(userId, pagination, cancellationToken);

        await Task.WhenAll(postsTask, eventsTask, purchasesTask, participationsTask);

        return new UserActivityHistory(
            await postsTask,
            await eventsTask,
            await purchasesTask,
            await participationsTask);
    }
}
