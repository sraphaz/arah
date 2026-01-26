using Araponga.Api.Contracts.Common;
using Araponga.Api.Contracts.Subscriptions;
using Araponga.Api.Security;
using Araponga.Application.Common;
using Araponga.Application.Services;
using Araponga.Domain.Subscriptions;
using Araponga.Domain.Users;
using Microsoft.AspNetCore.Mvc;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/subscriptions")]
[Produces("application/json")]
[Tags("Subscriptions")]
public sealed class SubscriptionsController : ControllerBase
{
    private readonly SubscriptionService _subscriptionService;
    private readonly CurrentUserAccessor _currentUserAccessor;

    public SubscriptionsController(
        SubscriptionService subscriptionService,
        CurrentUserAccessor currentUserAccessor)
    {
        _subscriptionService = subscriptionService;
        _currentUserAccessor = currentUserAccessor;
    }

    /// <summary>
    /// Obtém minha assinatura atual (retorna FREE se não tiver pago).
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(SubscriptionResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<SubscriptionResponse>> GetMySubscription(
        [FromQuery] Guid? territoryId,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var subscription = await _subscriptionService.GetUserSubscriptionAsync(
            userContext.User.Id,
            territoryId,
            cancellationToken);

        return Ok(await ToResponseAsync(subscription, cancellationToken));
    }

    /// <summary>
    /// Cria uma nova assinatura paga.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(SubscriptionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SubscriptionResponse>> Create(
        [FromBody] CreateSubscriptionRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _subscriptionService.CreateSubscriptionAsync(
            userContext.User.Id,
            request.TerritoryId,
            request.PlanId,
            request.CouponCode,
            cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new ErrorResponse { Message = result.Error });
        }

        return CreatedAtAction(
            nameof(Get),
            new { id = result.Value!.Id },
            await ToResponseAsync(result.Value, cancellationToken));
    }

    /// <summary>
    /// Obtém assinatura por ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SubscriptionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SubscriptionResponse>> Get(
        Guid id,
        CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionService.GetSubscriptionAsync(id, cancellationToken);
        if (subscription == null)
        {
            return NotFound();
        }

        // Verificar se o usuário tem acesso
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null || subscription.UserId != userContext.User.Id)
        {
            return Forbid();
        }

        return Ok(await ToResponseAsync(subscription, cancellationToken));
    }

    /// <summary>
    /// Atualiza assinatura (upgrade/downgrade).
    /// </summary>
    [HttpPatch("{id}")]
    [ProducesResponseType(typeof(SubscriptionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SubscriptionResponse>> Update(
        Guid id,
        [FromBody] UpdateSubscriptionRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        // Verificar se assinatura pertence ao usuário
        var subscription = await _subscriptionService.GetSubscriptionAsync(id, cancellationToken);
        if (subscription == null)
        {
            return NotFound();
        }

        if (subscription.UserId != userContext.User.Id)
        {
            return Forbid();
        }

        var result = await _subscriptionService.UpdateSubscriptionAsync(
            id,
            request.NewPlanId,
            cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new ErrorResponse { Message = result.Error });
        }

        return Ok(await ToResponseAsync(result.Value!, cancellationToken));
    }

    /// <summary>
    /// Cancela assinatura (volta para FREE).
    /// </summary>
    [HttpPost("{id}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Cancel(
        Guid id,
        [FromBody] CancelSubscriptionRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        // Verificar se assinatura pertence ao usuário
        var subscription = await _subscriptionService.GetSubscriptionAsync(id, cancellationToken);
        if (subscription == null)
        {
            return NotFound();
        }

        if (subscription.UserId != userContext.User.Id)
        {
            return Forbid();
        }

        var result = await _subscriptionService.CancelSubscriptionAsync(
            id,
            request.CancelAtPeriodEnd,
            cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new ErrorResponse { Message = result.Error });
        }

        return NoContent();
    }

    /// <summary>
    /// Reativa assinatura cancelada.
    /// </summary>
    [HttpPost("{id}/reactivate")]
    [ProducesResponseType(typeof(SubscriptionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SubscriptionResponse>> Reactivate(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        // Verificar se assinatura pertence ao usuário
        var subscription = await _subscriptionService.GetSubscriptionAsync(id, cancellationToken);
        if (subscription == null)
        {
            return NotFound();
        }

        if (subscription.UserId != userContext.User.Id)
        {
            return Forbid();
        }

        var result = await _subscriptionService.ReactivateSubscriptionAsync(id, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new ErrorResponse { Message = result.Error });
        }

        return Ok(await ToResponseAsync(result.Value!, cancellationToken));
    }

    private async Task<SubscriptionResponse> ToResponseAsync(Subscription subscription, CancellationToken cancellationToken)
    {
        var plan = await _subscriptionService.GetPlanByIdAsync(subscription.PlanId, cancellationToken);
        return new SubscriptionResponse
        {
            Id = subscription.Id,
            UserId = subscription.UserId,
            TerritoryId = subscription.TerritoryId,
            PlanId = subscription.PlanId,
            Tier = plan?.Tier.ToString() ?? SubscriptionPlanTier.FREE.ToString(),
            Status = subscription.Status.ToString(),
            CurrentPeriodStart = subscription.CurrentPeriodStart,
            CurrentPeriodEnd = subscription.CurrentPeriodEnd,
            TrialStart = subscription.TrialStart,
            TrialEnd = subscription.TrialEnd,
            CanceledAt = subscription.CanceledAt,
            CancelAtPeriodEnd = subscription.CancelAtPeriodEnd
        };
    }
}
