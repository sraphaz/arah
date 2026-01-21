using Araponga.Api.Contracts.Common;
using Araponga.Api.Contracts.Users;
using Araponga.Api.Security;
using Araponga.Application.Common;
using Araponga.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/users/me")]
[Produces("application/json")]
[Tags("User Activity")]
public sealed class UserActivityController : ControllerBase
{
    private readonly UserActivityService _activityService;
    private readonly CurrentUserAccessor _currentUserAccessor;

    public UserActivityController(
        UserActivityService activityService,
        CurrentUserAccessor currentUserAccessor)
    {
        _activityService = activityService;
        _currentUserAccessor = currentUserAccessor;
    }

    /// <summary>
    /// Obtém histórico completo de atividades do usuário autenticado.
    /// </summary>
    [HttpGet("activity")]
    [EnableRateLimiting("feed")]
    [ProducesResponseType(typeof(UserActivityHistoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserActivityHistoryResponse>> GetActivityHistory(
        CancellationToken cancellationToken,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var pagination = new PaginationParameters(pageNumber, pageSize);
        var history = await _activityService.GetUserActivityHistoryAsync(
            userContext.User.Id,
            pagination,
            cancellationToken);

        var response = new UserActivityHistoryResponse(
            ToPagedResponse(history.Posts, ToPostResponse),
            ToPagedResponse(history.Events, ToEventResponse),
            ToPagedResponse(history.Purchases, ToPurchaseResponse),
            ToPagedResponse(history.Participations, ToParticipationResponse));

        return Ok(response);
    }

    /// <summary>
    /// Obtém posts criados pelo usuário autenticado.
    /// </summary>
    [HttpGet("posts")]
    [EnableRateLimiting("feed")]
    [ProducesResponseType(typeof(PagedResponse<UserPostActivityResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResponse<UserPostActivityResponse>>> GetMyPosts(
        CancellationToken cancellationToken,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var pagination = new PaginationParameters(pageNumber, pageSize);
        var result = await _activityService.GetUserPostsAsync(
            userContext.User.Id,
            pagination,
            cancellationToken);

        var response = ToPagedResponse(result, ToPostResponse);
        return Ok(response);
    }

    /// <summary>
    /// Obtém eventos criados pelo usuário autenticado.
    /// </summary>
    [HttpGet("events")]
    [EnableRateLimiting("feed")]
    [ProducesResponseType(typeof(PagedResponse<UserEventActivityResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResponse<UserEventActivityResponse>>> GetMyEvents(
        CancellationToken cancellationToken,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var pagination = new PaginationParameters(pageNumber, pageSize);
        var result = await _activityService.GetUserEventsAsync(
            userContext.User.Id,
            pagination,
            cancellationToken);

        var response = ToPagedResponse(result, ToEventResponse);
        return Ok(response);
    }

    /// <summary>
    /// Obtém compras realizadas pelo usuário autenticado.
    /// </summary>
    [HttpGet("purchases")]
    [EnableRateLimiting("feed")]
    [ProducesResponseType(typeof(PagedResponse<UserPurchaseActivityResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResponse<UserPurchaseActivityResponse>>> GetMyPurchases(
        CancellationToken cancellationToken,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var pagination = new PaginationParameters(pageNumber, pageSize);
        var result = await _activityService.GetUserPurchasesAsync(
            userContext.User.Id,
            pagination,
            cancellationToken);

        var response = ToPagedResponse(result, ToPurchaseResponse);
        return Ok(response);
    }

    /// <summary>
    /// Obtém vendas realizadas pelo usuário autenticado (como dono de loja).
    /// </summary>
    [HttpGet("sales")]
    [EnableRateLimiting("feed")]
    [ProducesResponseType(typeof(PagedResponse<UserSaleActivityResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResponse<UserSaleActivityResponse>>> GetMySales(
        CancellationToken cancellationToken,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var pagination = new PaginationParameters(pageNumber, pageSize);
        var result = await _activityService.GetUserSalesAsync(
            userContext.User.Id,
            pagination,
            cancellationToken);

        var response = ToPagedResponse(result, ToSaleResponse);
        return Ok(response);
    }

    private static PagedResponse<TResponse> ToPagedResponse<TModel, TResponse>(
        Application.Common.PagedResult<TModel> pagedResult,
        Func<TModel, TResponse> mapper)
    {
        const int maxInt32 = int.MaxValue;
        var items = pagedResult.Items.Select(mapper).ToList();
        var safeTotalCount = pagedResult.TotalCount > maxInt32 ? maxInt32 : pagedResult.TotalCount;
        var safeTotalPages = pagedResult.TotalPages > maxInt32 ? maxInt32 : pagedResult.TotalPages;
        return new PagedResponse<TResponse>(
            items,
            pagedResult.PageNumber,
            pagedResult.PageSize,
            safeTotalCount,
            safeTotalPages,
            pagedResult.HasPreviousPage,
            pagedResult.HasNextPage);
    }

    private static UserPostActivityResponse ToPostResponse(Application.Models.UserPostActivity activity)
    {
        return new UserPostActivityResponse(
            activity.Id,
            activity.TerritoryId,
            activity.Title,
            activity.Type,
            activity.Status,
            activity.CreatedAtUtc,
            activity.EditedAtUtc,
            activity.EditCount);
    }

    private static UserEventActivityResponse ToEventResponse(Application.Models.UserEventActivity activity)
    {
        return new UserEventActivityResponse(
            activity.Id,
            activity.TerritoryId,
            activity.Title,
            activity.StartsAtUtc,
            activity.Status,
            activity.CreatedAtUtc);
    }

    private static UserPurchaseActivityResponse ToPurchaseResponse(Application.Models.UserPurchaseActivity activity)
    {
        return new UserPurchaseActivityResponse(
            activity.Id,
            activity.TerritoryId,
            activity.TotalAmount,
            activity.Currency,
            activity.Status,
            activity.CreatedAtUtc);
    }

    private static UserSaleActivityResponse ToSaleResponse(Application.Models.UserSaleActivity activity)
    {
        return new UserSaleActivityResponse(
            activity.Id,
            activity.TerritoryId,
            activity.TotalAmount,
            activity.Currency,
            activity.Status,
            activity.CreatedAtUtc);
    }

    private static UserParticipationActivityResponse ToParticipationResponse(Application.Models.UserParticipationActivity activity)
    {
        return new UserParticipationActivityResponse(
            activity.EventId,
            activity.TerritoryId,
            activity.EventTitle,
            activity.StartsAtUtc,
            activity.Status,
            activity.CreatedAtUtc);
    }
}
