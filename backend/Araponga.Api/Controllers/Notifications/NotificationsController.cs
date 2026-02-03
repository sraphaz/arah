using Araponga.Api.Contracts.Common;
using Araponga.Api.Contracts.Notifications;
using Araponga.Api.Security;
using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/notifications")]
[Produces("application/json")]
[Tags("Notifications")]
public sealed class NotificationsController : ControllerBase
{
    private readonly INotificationInboxRepository _notificationRepository;
    private readonly CurrentUserAccessor _currentUserAccessor;

    public NotificationsController(
        INotificationInboxRepository notificationRepository,
        CurrentUserAccessor currentUserAccessor)
    {
        _notificationRepository = notificationRepository;
        _currentUserAccessor = currentUserAccessor;
    }

    /// <summary>
    /// Lista as notificações in-app do usuário autenticado.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<NotificationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<NotificationResponse>>> List(
        [FromQuery] int skip = 0,
        [FromQuery] int take = Constants.Pagination.DefaultPageSize,
        CancellationToken cancellationToken = default)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        if (skip < 0)
        {
            skip = 0;
        }

        if (take <= 0 || take > Constants.Pagination.MaxPageSize)
        {
            take = Constants.Pagination.DefaultPageSize;
        }

        var notifications = await _notificationRepository.ListByUserAsync(
            userContext.User.Id,
            skip,
            take,
            cancellationToken);

        var response = notifications.Select(notification => new NotificationResponse(
            notification.Id,
            notification.Title,
            notification.Body,
            notification.Kind,
            notification.DataJson,
            notification.CreatedAtUtc,
            notification.ReadAtUtc));

        return Ok(response);
    }

    /// <summary>
    /// Lista as notificações in-app do usuário autenticado (paginado).
    /// </summary>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(PagedResponse<NotificationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResponse<NotificationResponse>>> ListPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = Constants.Pagination.DefaultPageSize,
        CancellationToken cancellationToken = default)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var pagination = new PaginationParameters(pageNumber, pageSize);
        var totalCount = await _notificationRepository.CountByUserAsync(userContext.User.Id, cancellationToken);
        var notifications = await _notificationRepository.ListByUserAsync(
            userContext.User.Id,
            pagination.Skip,
            pagination.Take,
            cancellationToken);

        var items = notifications.Select(notification => new NotificationResponse(
            notification.Id,
            notification.Title,
            notification.Body,
            notification.Kind,
            notification.DataJson,
            notification.CreatedAtUtc,
            notification.ReadAtUtc)).ToList();

        const int maxInt32 = int.MaxValue;
        var safeTotalCount = totalCount > maxInt32 ? maxInt32 : totalCount;
        var totalPagesDouble = Math.Ceiling(safeTotalCount / (double)pagination.PageSize);
        var safeTotalPages = totalPagesDouble > maxInt32 ? maxInt32 : (int)totalPagesDouble;
        var response = new PagedResponse<NotificationResponse>(
            items,
            pagination.PageNumber,
            pagination.PageSize,
            safeTotalCount,
            safeTotalPages,
            pagination.PageNumber > 1,
            pagination.PageNumber < safeTotalPages);

        return Ok(response);
    }

    /// <summary>
    /// Marca uma notificação como lida.
    /// </summary>
    [HttpPost("{id:guid}/read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsRead(Guid id, CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var updated = await _notificationRepository.MarkAsReadAsync(
            id,
            userContext.User.Id,
            DateTime.UtcNow,
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }
}
