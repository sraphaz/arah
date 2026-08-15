using Arah.Api.Contracts.Common;
using Arah.Api.Contracts.Subscriptions;
using Arah.Api.Contracts.Transactions;
using Arah.Api.Security;
using Arah.Application.Services;
using Arah.Domain.Users;
using Microsoft.AspNetCore.Mvc;

namespace Arah.Api.Controllers;

/// <summary>
/// Alias merchant (store) — FASE55: subscription + consumption.
/// </summary>
[ApiController]
[Route("api/v1/merchants")]
[Produces("application/json")]
[Tags("Merchants — FASE55")]
public sealed class MerchantsController : ControllerBase
{
    private readonly MerchantCommercialService _merchants;
    private readonly CurrentUserAccessor _currentUserAccessor;
    private readonly SubscriptionService _subscriptionService;

    public MerchantsController(
        MerchantCommercialService merchants,
        CurrentUserAccessor currentUserAccessor,
        SubscriptionService subscriptionService)
    {
        _merchants = merchants;
        _currentUserAccessor = currentUserAccessor;
        _subscriptionService = subscriptionService;
    }

    /// <summary>
    /// Alias de POST /subscriptions para o comércio (store id = merchant id). AC-55-9
    /// </summary>
    [HttpPost("{merchantId:guid}/subscription")]
    [ProducesResponseType(typeof(SubscriptionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SubscriptionResponse>> CreateSubscription(
        Guid merchantId,
        [FromBody] CreateMerchantSubscriptionRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _merchants.CreateSubscriptionForMerchantAsync(
            merchantId,
            userContext.User.Id,
            request.PlanId,
            request.CouponCode,
            cancellationToken);

        if (result.IsFailure)
        {
            return MapFailure(result.Error);
        }

        var created = result.Value!;
        var plan = await _subscriptionService.GetPlanByIdAsync(created.PlanId, cancellationToken);
        var response = new SubscriptionResponse
        {
            Id = created.Id,
            UserId = created.UserId,
            TerritoryId = created.TerritoryId,
            PlanId = created.PlanId,
            Tier = plan?.Tier.ToString() ?? "UNKNOWN",
            Status = created.Status.ToString(),
            CurrentPeriodStart = created.CurrentPeriodStart,
            CurrentPeriodEnd = created.CurrentPeriodEnd,
            TrialStart = created.TrialStart,
            TrialEnd = created.TrialEnd,
            CanceledAt = created.CanceledAt,
            CancelAtPeriodEnd = created.CancelAtPeriodEnd
        };

        return Created($"/api/v1/subscriptions/{created.Id}", response);
    }

    /// <summary>
    /// Consumo medido do comércio (v0: zeros até writers). AC-55-11
    /// </summary>
    [HttpGet("{merchantId:guid}/consumption")]
    [ProducesResponseType(typeof(IReadOnlyList<ConsumptionMeterResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<ConsumptionMeterResponse>>> GetConsumption(
        Guid merchantId,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _merchants.GetConsumptionAsync(
            merchantId,
            userContext.User.Id,
            cancellationToken);

        if (result.IsFailure)
        {
            return MapFailure(result.Error);
        }

        var body = result.Value!
            .Select(m => new ConsumptionMeterResponse(
                m.Id,
                m.SubscriptionId,
                m.Metric,
                m.Usage,
                m.Quota,
                m.OverageRate))
            .ToList();

        return Ok(body);
    }

    private ActionResult MapFailure(string? error)
    {
        if (string.Equals(error, "Merchant not found.", StringComparison.Ordinal))
        {
            return NotFound(new ErrorResponse { Message = error });
        }

        if (string.Equals(error, "Forbidden.", StringComparison.Ordinal))
        {
            return Forbid();
        }

        return BadRequest(new ErrorResponse { Message = error ?? "Request failed." });
    }
}
