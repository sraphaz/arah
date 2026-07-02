using Arah.Api.Contracts.Transactions;
using Arah.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Arah.Api.Controllers;

[ApiController]
[Route("api/v1/territories/{territoryId:guid}/payouts")]
[Produces("application/json")]
[Tags("Payouts — FASE55")]
public sealed class TerritoryPayoutsController : ControllerBase
{
    private readonly PayoutConsolidationService _payouts;

    public TerritoryPayoutsController(PayoutConsolidationService payouts)
    {
        _payouts = payouts;
    }

    /// <summary>
    /// Payout consolidado por período (AC-55-5) — agrega o split das transações pagas.
    /// </summary>
    [HttpGet("consolidated")]
    [ProducesResponseType(typeof(ConsolidatedPayoutResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ConsolidatedPayoutResponse>> GetConsolidated(
        Guid territoryId,
        [FromQuery] DateTimeOffset from,
        [FromQuery] DateTimeOffset to,
        CancellationToken cancellationToken)
    {
        var result = await _payouts.GetConsolidatedAsync(territoryId, from, to, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error ?? "Consolidation failed." });
        }

        var payout = result.Value!;
        return Ok(new ConsolidatedPayoutResponse(
            payout.TerritoryId,
            payout.FromUtc,
            payout.ToUtc,
            payout.Currency,
            payout.TransactionCount,
            payout.TotalGrossAmount,
            payout.TotalFeeAmount,
            payout.Lines.Select(l => new ConsolidatedPayoutLineResponse(l.Recipient, l.Amount)).ToList()));
    }
}
