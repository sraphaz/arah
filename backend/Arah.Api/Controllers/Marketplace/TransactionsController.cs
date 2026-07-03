using Arah.Api.Contracts.Transactions;
using Arah.Application.Interfaces;
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
    private readonly PaymentService _payments;

    public TransactionsController(
        TransactionQuoteService quotes,
        RefundService refunds,
        PaymentService payments)
    {
        _quotes = quotes;
        _refunds = refunds;
        _payments = payments;
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

    [HttpPost("{transactionId:guid}/pay")]
    [ProducesResponseType(typeof(TransactionPayResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TransactionPayResponse>> Pay(
        Guid transactionId,
        [FromBody] TransactionPayRequest? request,
        CancellationToken cancellationToken)
    {
        var method = ParsePaymentMethod(request?.Method);
        var result = await _payments.InitiatePaymentAsync(transactionId, method, cancellationToken);
        if (result.IsFailure)
        {
            var message = result.Error ?? "Payment initiation failed.";
            return message.Contains("not found", StringComparison.OrdinalIgnoreCase)
                ? NotFound(new { error = message })
                : BadRequest(new { error = message });
        }

        var payment = result.Value!;
        return Ok(new TransactionPayResponse(
            payment.TransactionId,
            payment.Status.ToString(),
            payment.GatewayPaymentId,
            payment.Method.ToString().ToLowerInvariant(),
            payment.PixCopyPasteCode,
            payment.InitiatedAtUtc));
    }

    [HttpPost("{transactionId:guid}/confirm-payment")]
    [ProducesResponseType(typeof(TransactionConfirmPaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TransactionConfirmPaymentResponse>> ConfirmPayment(
        Guid transactionId,
        [FromBody] TransactionConfirmPaymentRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.GatewayPaymentId))
        {
            return BadRequest(new { error = "GatewayPaymentId is required." });
        }

        var result = await _payments.ConfirmPaymentAsync(
            transactionId,
            request.GatewayPaymentId,
            cancellationToken);

        if (result.IsFailure)
        {
            var message = result.Error ?? "Payment confirmation failed.";
            return message.Contains("not found", StringComparison.OrdinalIgnoreCase)
                ? NotFound(new { error = message })
                : BadRequest(new { error = message });
        }

        var confirm = result.Value!;
        return Ok(new TransactionConfirmPaymentResponse(
            confirm.TransactionId,
            confirm.Status.ToString(),
            confirm.GatewayPaymentId,
            confirm.PaidAtUtc,
            confirm.AlreadyPaid));
    }

    private static PaymentMethod ParsePaymentMethod(string? method) =>
        method?.Trim().ToLowerInvariant() switch
        {
            "stripe" => PaymentMethod.Stripe,
            _ => PaymentMethod.Pix
        };

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
