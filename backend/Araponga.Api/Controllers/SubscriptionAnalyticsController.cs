using Araponga.Api.Contracts.Subscriptions;
using Araponga.Api.Security;
using Araponga.Application.Services;
using Araponga.Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Araponga.Api.Controllers;

/// <summary>
/// Controller para analytics e métricas de assinaturas.
/// </summary>
[ApiController]
[Route("api/v1/admin/subscriptions/analytics")]
[Produces("application/json")]
[Tags("Admin - Subscriptions Analytics")]
[Authorize(Policy = "SystemAdmin")]
public sealed class SubscriptionAnalyticsController : ControllerBase
{
    private readonly SubscriptionAnalyticsService _analyticsService;

    public SubscriptionAnalyticsController(SubscriptionAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    /// <summary>
    /// Obtém métricas gerais de assinaturas.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(SubscriptionAnalyticsResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<SubscriptionAnalyticsResponse>> GetAnalytics(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        CancellationToken cancellationToken)
    {
        var mrr = await _analyticsService.GetMRRAsync(startDate, endDate, cancellationToken);
        var churnRate = await _analyticsService.GetChurnRateAsync(startDate, endDate, cancellationToken);
        var activeCount = await _analyticsService.GetActiveSubscriptionsCountAsync(cancellationToken);
        var newCount = await _analyticsService.GetNewSubscriptionsCountAsync(startDate, endDate, cancellationToken);
        var canceledCount = await _analyticsService.GetCanceledSubscriptionsCountAsync(startDate, endDate, cancellationToken);
        var revenueByPlan = await _analyticsService.GetRevenueByPlanAsync(startDate, endDate, cancellationToken);

        var response = new SubscriptionAnalyticsResponse
        {
            MRR = mrr,
            ChurnRate = churnRate,
            ActiveSubscriptionsCount = activeCount,
            NewSubscriptionsCount = newCount,
            CanceledSubscriptionsCount = canceledCount,
            RevenueByPlan = revenueByPlan,
            PeriodStart = startDate ?? DateTime.UtcNow.AddMonths(-1),
            PeriodEnd = endDate ?? DateTime.UtcNow
        };

        return Ok(response);
    }

    /// <summary>
    /// Obtém o MRR (Monthly Recurring Revenue).
    /// </summary>
    [HttpGet("mrr")]
    [ProducesResponseType(typeof(MRRResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<MRRResponse>> GetMRR(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        CancellationToken cancellationToken)
    {
        var mrr = await _analyticsService.GetMRRAsync(startDate, endDate, cancellationToken);

        return Ok(new MRRResponse
        {
            MRR = mrr,
            PeriodStart = startDate ?? DateTime.UtcNow.AddMonths(-1),
            PeriodEnd = endDate ?? DateTime.UtcNow
        });
    }

    /// <summary>
    /// Obtém a taxa de churn.
    /// </summary>
    [HttpGet("churn")]
    [ProducesResponseType(typeof(ChurnRateResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ChurnRateResponse>> GetChurnRate(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        CancellationToken cancellationToken)
    {
        var churnRate = await _analyticsService.GetChurnRateAsync(startDate, endDate, cancellationToken);

        return Ok(new ChurnRateResponse
        {
            ChurnRate = churnRate,
            PeriodStart = startDate ?? DateTime.UtcNow.AddMonths(-1),
            PeriodEnd = endDate ?? DateTime.UtcNow
        });
    }

    /// <summary>
    /// Obtém receita por plano no período.
    /// </summary>
    [HttpGet("revenue")]
    [ProducesResponseType(typeof(RevenueByPlanResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<RevenueByPlanResponse>> GetRevenueByPlan(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        CancellationToken cancellationToken)
    {
        var revenueByPlan = await _analyticsService.GetRevenueByPlanAsync(startDate, endDate, cancellationToken);

        return Ok(new RevenueByPlanResponse
        {
            RevenueByPlan = revenueByPlan,
            PeriodStart = startDate ?? DateTime.UtcNow.AddMonths(-1),
            PeriodEnd = endDate ?? DateTime.UtcNow
        });
    }
}
