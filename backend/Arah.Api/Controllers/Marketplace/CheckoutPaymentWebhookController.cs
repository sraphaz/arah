using Arah.Application.Interfaces;
using Arah.Application.Services;
using Arah.Infrastructure.Payments;
using Microsoft.AspNetCore.Mvc;

namespace Arah.Api.Controllers;

/// <summary>
/// Webhook mock para simular confirmação de pagamento de checkout (dev/testes — DOD-05).
/// Em produção, substituir por handlers Stripe/Mercado Pago reais.
/// </summary>
[ApiController]
[Route("api/v1/webhooks/checkout")]
[Produces("application/json")]
[Tags("Webhooks")]
public sealed class CheckoutPaymentWebhookController : ControllerBase
{
    private readonly PaymentService _paymentService;
    private readonly IWebHostEnvironment _environment;
    private readonly IPaymentGateway _paymentGateway;

    public CheckoutPaymentWebhookController(
        PaymentService paymentService,
        IWebHostEnvironment environment,
        IPaymentGateway paymentGateway)
    {
        _paymentService = paymentService;
        _environment = environment;
        _paymentGateway = paymentGateway;
    }

    [HttpPost("mock-approved")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MockApproved(
        [FromBody] MockCheckoutPaymentWebhookRequest request,
        CancellationToken cancellationToken)
    {
        if (!_environment.IsDevelopment() && !_environment.IsEnvironment("Testing"))
        {
            return NotFound();
        }

        if (_paymentGateway is not MockPaymentGateway mockGateway)
        {
            return BadRequest(new { error = "Mock webhook requires MockPaymentGateway." });
        }

        if (request.CheckoutId == Guid.Empty || string.IsNullOrWhiteSpace(request.GatewayPaymentId))
        {
            return BadRequest(new { error = "checkoutId and gatewayPaymentId are required." });
        }

        mockGateway.SimulatePaymentApproved(request.GatewayPaymentId);

        var result = await _paymentService.ConfirmPaymentAsync(
            request.CheckoutId,
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
        return Ok(new
        {
            transactionId = confirm.TransactionId,
            status = confirm.Status.ToString(),
            alreadyPaid = confirm.AlreadyPaid
        });
    }
}

public sealed record MockCheckoutPaymentWebhookRequest(Guid CheckoutId, string GatewayPaymentId);
