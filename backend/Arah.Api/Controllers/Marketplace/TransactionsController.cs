using Arah.Api.Contracts.Transactions;
using Arah.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Arah.Api.Controllers;

[ApiController]
[Route("api/v1/transactions")]
[Produces("application/json")]
[Tags("Transactions — FASE55")]
public sealed class TransactionsController : ControllerBase
{
    private readonly TransactionQuoteService _quotes;
    private readonly RefundService _refunds;

    public TransactionsController(TransactionQuoteService quotes, RefundService refunds)
    {
        _quotes = quotes;
        _refunds = refunds;
    }

    [HttpPost("{transactionId:guid}/quote")]
    [ProducesResponseType(typeof(TransactionQuoteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TransactionQuoteResponse>> Quote(
        Guid transactionId,
        CancellationToken cancellationToken)
    {
        var result = await _quotes.QuoteAsync(transactionId, cancellationToken);
        if (result.IsFailure)
        {
            var message = result.Error ?? "Quote failed.";
            return message.Contains("not found", StringComparison.OrdinalIgnoreCase)
                ? NotFound(new { error = message })
                : BadRequest(new { error = message });
        }

        return Ok(ToQuoteResponse(result.Value!));
    }

    [HttpGet("{transactionId:guid}/receipt")]
    [ProducesResponseType(typeof(TransactionReceiptResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TransactionReceiptResponse>> Receipt(
        Guid transactionId,
        CancellationToken cancellationToken)
    {
        var result = await _quotes.GetReceiptAsync(transactionId, cancellationToken);
        if (result.IsFailure)
        {
            var message = result.Error ?? "Receipt unavailable.";
            return message.Contains("not found", StringComparison.OrdinalIgnoreCase)
                ? NotFound(new { error = message })
                : BadRequest(new { error = message });
        }

        var receipt = result.Value!;
        return Ok(new TransactionReceiptResponse(
            receipt.TransactionId,
            receipt.Status.ToString(),
            receipt.Currency,
            receipt.GrossAmount,
            receipt.FeeAmount,
            receipt.NetToSeller,
            receipt.Split.Select(s => new FeeSplitLineResponse(s.Recipient, s.Amount)).ToList(),
            receipt.FeeSplitRuleId,
            receipt.QuotedAtUtc,
            receipt.PaidAtUtc));
    }

    [HttpPost("{transactionId:guid}/refund")]
    [ProducesResponseType(typeof(TransactionRefundResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TransactionRefundResponse>> Refund(
        Guid transactionId,
        CancellationToken cancellationToken)
    {
        var result = await _refunds.RefundAsync(transactionId, cancellationToken);
        if (result.IsFailure)
        {
            var message = result.Error ?? "Refund failed.";
            if (message.Contains("not found", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound(new { error = message });
            }

            return message.Contains("already refunded", StringComparison.OrdinalIgnoreCase)
                ? Conflict(new { error = message })
                : BadRequest(new { error = message });
        }

        var refund = result.Value!;
        return Ok(new TransactionRefundResponse(
            refund.TransactionId,
            refund.Status.ToString(),
            refund.Currency,
            refund.ReversedGrossAmount,
            refund.ReversedFeeAmount,
            refund.ReversedNetToSeller,
            refund.ReversedSplit.Select(s => new FeeSplitLineResponse(s.Recipient, s.Amount)).ToList(),
            refund.FeeSplitRuleId,
            refund.RefundedAtUtc));
    }

    private static TransactionQuoteResponse ToQuoteResponse(TransactionQuoteResult quote) =>
        new(
            quote.TransactionId,
            quote.TerritoryId,
            quote.StoreId,
            quote.Currency,
            quote.GrossAmount,
            quote.FeeAmount,
            quote.NetToSeller,
            quote.Split.Select(s => new FeeSplitLineResponse(s.Recipient, s.Amount)).ToList(),
            quote.FeeSplitRuleId,
            quote.QuotedAtUtc);
}
