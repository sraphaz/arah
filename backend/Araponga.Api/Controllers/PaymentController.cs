using Araponga.Api.Contracts.Payments;
using Araponga.Api.Security;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Text.RegularExpressions;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/payments")]
[Produces("application/json")]
[Tags("Payments")]
public sealed class PaymentController : ControllerBase
{
    private readonly PaymentService _paymentService;
    private readonly CurrentUserAccessor _currentUserAccessor;
    private readonly InputSanitizationService _sanitizationService;
    private readonly ILogger<PaymentController> _logger;

    // Whitelist de gateways permitidos
    private static readonly HashSet<string> AllowedGateways = new(StringComparer.OrdinalIgnoreCase)
    {
        "stripe", "mercadopago", "pagseguro", "mock"
    };

    // Moedas suportadas
    private static readonly HashSet<string> SupportedCurrencies = new(StringComparer.OrdinalIgnoreCase)
    {
        "BRL", "USD", "EUR"
    };

    public PaymentController(
        PaymentService paymentService,
        CurrentUserAccessor currentUserAccessor,
        InputSanitizationService sanitizationService,
        ILogger<PaymentController> logger)
    {
        _paymentService = paymentService;
        _currentUserAccessor = currentUserAccessor;
        _sanitizationService = sanitizationService;
        _logger = logger;
    }

    /// <summary>
    /// Cria um pagamento para um checkout.
    /// </summary>
    [HttpPost("create")]
    [EnableRateLimiting("write")]
    [ProducesResponseType(typeof(CreatePaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<CreatePaymentResponse>> CreatePayment(
        [FromBody] CreatePaymentRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        if (request.CheckoutId == Guid.Empty)
        {
            return BadRequest(new { error = "checkoutId is required." });
        }

        // Sanitizar e validar returnUrl
        string? sanitizedReturnUrl = null;
        if (!string.IsNullOrWhiteSpace(request.ReturnUrl))
        {
            sanitizedReturnUrl = _sanitizationService.SanitizeUrl(request.ReturnUrl);
            if (sanitizedReturnUrl is null)
            {
                return BadRequest(new { error = "Invalid returnUrl format." });
            }
        }

        // Sanitizar e validar metadata
        Dictionary<string, string>? sanitizedMetadata = null;
        if (request.Metadata is not null)
        {
            if (request.Metadata.Count > 20)
            {
                return BadRequest(new { error = "Metadata cannot contain more than 20 entries." });
            }

            sanitizedMetadata = new Dictionary<string, string>();
            foreach (var (key, value) in request.Metadata)
            {
                if (key.Length > 40 || value.Length > 500)
                {
                    return BadRequest(new { error = "Metadata keys/values exceed maximum length (key: 40, value: 500)." });
                }

                var sanitizedKey = _sanitizationService.SanitizeText(key);
                var sanitizedValue = _sanitizationService.SanitizeText(value);
                
                if (string.IsNullOrWhiteSpace(sanitizedKey))
                {
                    return BadRequest(new { error = "Metadata keys cannot be empty after sanitization." });
                }

                sanitizedMetadata[sanitizedKey] = sanitizedValue;
            }
        }

        _logger.LogInformation(
            "Creating payment for checkout {CheckoutId} by user {UserId}",
            request.CheckoutId, userContext.User.Id);

        var result = await _paymentService.CreatePaymentAsync(
            request.CheckoutId,
            userContext.User.Id,
            sanitizedReturnUrl,
            sanitizedMetadata,
            cancellationToken);

        if (result.IsFailure)
        {
            _logger.LogWarning(
                "Payment creation failed for checkout {CheckoutId}: {Error}",
                request.CheckoutId, result.Error);
            return BadRequest(new { error = result.Error ?? "Unable to create payment." });
        }

        var payment = result.Value!;
        _logger.LogInformation(
            "Payment created successfully: PaymentIntentId {PaymentIntentId}, CheckoutId {CheckoutId}",
            payment.PaymentIntentId, payment.CheckoutId);

        return Ok(new CreatePaymentResponse(
            payment.PaymentIntentId,
            payment.PaymentUrl,
            payment.ClientSecret,
            payment.CheckoutId));
    }

    /// <summary>
    /// Confirma um pagamento após o usuário completar o fluxo no gateway.
    /// </summary>
    [HttpPost("confirm")]
    [EnableRateLimiting("write")]
    [ProducesResponseType(typeof(ConfirmPaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<ConfirmPaymentResponse>> ConfirmPayment(
        [FromBody] ConfirmPaymentRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        if (string.IsNullOrWhiteSpace(request.PaymentIntentId))
        {
            return BadRequest(new { error = "paymentIntentId is required." });
        }

        // Validar formato do PaymentIntentId
        if (!IsValidPaymentIntentId(request.PaymentIntentId))
        {
            return BadRequest(new { error = "Invalid paymentIntentId format." });
        }

        var sanitizedPaymentIntentId = request.PaymentIntentId
            .Replace("\r", string.Empty)
            .Replace("\n", string.Empty);

        _logger.LogInformation(
            "Confirming payment {PaymentIntentId} by user {UserId}",
            sanitizedPaymentIntentId, userContext.User.Id);

        var result = await _paymentService.ConfirmPaymentAsync(
            request.PaymentIntentId,
            userContext.User.Id,
            cancellationToken);

        if (result.IsFailure)
        {
            _logger.LogWarning(
                "Payment confirmation failed for {PaymentIntentId}: {Error}",
                sanitizedPaymentIntentId, result.Error);
            return BadRequest(new { error = result.Error ?? "Unable to confirm payment." });
        }

        var confirmation = result.Value!;
        _logger.LogInformation(
            "Payment confirmed: PaymentIntentId {PaymentIntentId}, Status {Status}",
            confirmation.PaymentIntentId, confirmation.PaymentStatus);

        return Ok(new ConfirmPaymentResponse(
            confirmation.PaymentIntentId,
            confirmation.PaymentStatus.ToString(),
            confirmation.CheckoutId));
    }

    /// <summary>
    /// Cria um reembolso para um checkout pago.
    /// </summary>
    [HttpPost("refund")]
    [EnableRateLimiting("write")]
    [ProducesResponseType(typeof(CreateRefundResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<CreateRefundResponse>> CreateRefund(
        [FromBody] CreateRefundRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        if (request.CheckoutId == Guid.Empty)
        {
            return BadRequest(new { error = "checkoutId is required." });
        }

        // Validar amount se fornecido
        if (request.Amount.HasValue)
        {
            if (request.Amount.Value <= 0)
            {
                return BadRequest(new { error = "Refund amount must be positive." });
            }
        }

        // Sanitizar reason
        string? sanitizedReason = null;
        if (!string.IsNullOrWhiteSpace(request.Reason))
        {
            sanitizedReason = _sanitizationService.SanitizeText(request.Reason);
            if (sanitizedReason.Length > 500)
            {
                sanitizedReason = sanitizedReason.Substring(0, 500);
            }
        }

        _logger.LogInformation(
            "Creating refund for checkout {CheckoutId} by user {UserId}, amount: {Amount}",
            request.CheckoutId, userContext.User.Id, request.Amount);

        var result = await _paymentService.CreateRefundAsync(
            request.CheckoutId,
            userContext.User.Id,
            request.Amount,
            sanitizedReason,
            cancellationToken);

        if (result.IsFailure)
        {
            _logger.LogWarning(
                "Refund creation failed for checkout {CheckoutId}: {Error}",
                request.CheckoutId, result.Error);
            return BadRequest(new { error = result.Error ?? "Unable to create refund." });
        }

        var refund = result.Value!;
        _logger.LogInformation(
            "Refund created successfully: RefundId {RefundId}, Amount {Amount}, CheckoutId {CheckoutId}",
            refund.RefundId, refund.Amount, refund.CheckoutId);

        return Ok(new CreateRefundResponse(
            refund.RefundId,
            refund.Amount,
            refund.Status.ToString(),
            refund.CheckoutId));
    }

    /// <summary>
    /// Webhook para receber notificações do gateway de pagamento.
    /// IMPORTANTE: Em produção, validar assinatura do gateway antes de processar.
    /// </summary>
    [HttpPost("webhook")]
    [EnableRateLimiting("payment-webhook")]
    [IgnoreAntiforgeryToken] // Webhooks não usam CSRF tokens, mas validam assinatura
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> ProcessWebhook(
        [FromHeader(Name = "X-Signature")] string? signature,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(signature))
        {
            _logger.LogWarning("Webhook received without signature header");
            return BadRequest(new { error = "X-Signature header is required." });
        }

        string payload;
        using (var reader = new StreamReader(Request.Body))
        {
            payload = await reader.ReadToEndAsync(cancellationToken);
        }

        if (string.IsNullOrWhiteSpace(payload))
        {
            _logger.LogWarning("Webhook received with empty payload");
            return BadRequest(new { error = "Webhook payload is required." });
        }

        // Validar tamanho do payload (prevenir DoS)
        if (payload.Length > 100_000) // 100KB máximo
        {
            _logger.LogWarning("Webhook payload too large: {Size} bytes", payload.Length);
            return BadRequest(new { error = "Webhook payload exceeds maximum size." });
        }

        _logger.LogInformation(
            "Processing webhook with signature length {SignatureLength}, payload length {PayloadLength}",
            signature.Length, payload.Length);

        var result = await _paymentService.ProcessWebhookAsync(payload, signature, cancellationToken);

        if (result.IsFailure)
        {
            _logger.LogWarning("Webhook processing failed: {Error}", result.Error);
            return BadRequest(new { error = result.Error ?? "Unable to process webhook." });
        }

        _logger.LogInformation("Webhook processed successfully");
        return Ok(new { message = "Webhook processed successfully." });
    }

    /// <summary>
    /// Valida formato do PaymentIntentId.
    /// </summary>
    private static bool IsValidPaymentIntentId(string paymentIntentId)
    {
        if (string.IsNullOrWhiteSpace(paymentIntentId))
        {
            return false;
        }

        // Validar tamanho (10-200 caracteres)
        if (paymentIntentId.Length < 10 || paymentIntentId.Length > 200)
        {
            return false;
        }

        // Validar caracteres permitidos (alphanumeric, underscore, hyphen, dot)
        // Gateways comuns: Stripe (pi_xxx), MercadoPago (números), PagSeguro (alfanumérico)
        return Regex.IsMatch(paymentIntentId, @"^[a-zA-Z0-9_.-]+$");
    }
}
