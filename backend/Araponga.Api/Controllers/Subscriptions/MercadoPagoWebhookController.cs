using Araponga.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/webhooks/mercadopago")]
[Produces("application/json")]
[Tags("Webhooks")]
public sealed class MercadoPagoWebhookController : ControllerBase
{
    private readonly MercadoPagoWebhookService _webhookService;
    private readonly ILogger<MercadoPagoWebhookController> _logger;
    private readonly IConfiguration _configuration;

    public MercadoPagoWebhookController(
        MercadoPagoWebhookService webhookService,
        ILogger<MercadoPagoWebhookController> logger,
        IConfiguration configuration)
    {
        _webhookService = webhookService;
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Recebe webhooks do Mercado Pago para eventos de assinaturas.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> HandleWebhook(
        CancellationToken cancellationToken)
    {
        try
        {
            // Ler o body do request
            using var reader = new StreamReader(Request.Body, Encoding.UTF8);
            var body = await reader.ReadToEndAsync(cancellationToken);

            // Validar assinatura do Mercado Pago
            var mercadoPagoSignature = Request.Headers["X-Signature"].FirstOrDefault() ??
                                       Request.Headers["x-signature"].FirstOrDefault();
            var mercadoPagoRequestId = Request.Headers["X-Request-Id"].FirstOrDefault() ??
                                       Request.Headers["x-request-id"].FirstOrDefault();

            if (!ValidateMercadoPagoSignature(body, mercadoPagoSignature, mercadoPagoRequestId))
            {
                _logger.LogWarning("Invalid Mercado Pago webhook signature");
                return BadRequest("Invalid signature");
            }
            
            _logger.LogInformation("Received Mercado Pago webhook");

            // Parse do JSON do evento
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;

            // Mercado Pago envia eventos de diferentes formas
            // Pode ser um objeto direto ou dentro de "data"
            var eventData = root;
            string? eventType = null;

            // Tentar extrair tipo do evento
            if (root.TryGetProperty("type", out var typeProp))
            {
                eventType = typeProp.GetString();
            }
            else if (root.TryGetProperty("action", out var actionProp))
            {
                eventType = actionProp.GetString();
            }
            else if (root.TryGetProperty("data", out var dataProp))
            {
                eventData = dataProp;
                if (dataProp.TryGetProperty("type", out var dataTypeProp))
                {
                    eventType = dataTypeProp.GetString();
                }
            }

            if (string.IsNullOrEmpty(eventType))
            {
                _logger.LogWarning("Mercado Pago webhook received without event type");
                return BadRequest("Event type is required");
            }

            // Processar evento
            var result = await _webhookService.ProcessEventAsync(
                eventType,
                eventData,
                cancellationToken);

            if (result.IsFailure)
            {
                _logger.LogError("Error processing Mercado Pago webhook: {Error}", result.Error);
                return BadRequest(new { error = result.Error });
            }

            return Ok(new { received = true, eventType });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling Mercado Pago webhook");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Valida a assinatura do webhook do Mercado Pago.
    /// O Mercado Pago usa HMAC-SHA256 com o formato: sha256=hash
    /// </summary>
    private bool ValidateMercadoPagoSignature(string payload, string? signature, string? requestId)
    {
        if (string.IsNullOrEmpty(signature))
        {
            _logger.LogWarning("Mercado Pago webhook signature header is missing");
            return false;
        }

        var webhookSecret = _configuration["MercadoPago:WebhookSecret"];
        if (string.IsNullOrEmpty(webhookSecret))
        {
            // Em desenvolvimento, permitir sem validação se não houver secret configurado
            var environment = _configuration["ASPNETCORE_ENVIRONMENT"];
            if (string.IsNullOrEmpty(environment) || 
                environment.Equals("Development", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Mercado Pago webhook secret not configured. Skipping validation in development.");
                return true;
            }
            
            _logger.LogError("Mercado Pago webhook secret not configured in production");
            return false;
        }

        try
        {
            // Mercado Pago envia a assinatura no formato: sha256=hash
            if (!signature.StartsWith("sha256=", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Invalid Mercado Pago signature format. Expected 'sha256=...'");
                return false;
            }

            var receivedHash = signature.Substring(7); // Remove "sha256="

            // Calcular hash esperado
            // Mercado Pago usa: HMAC-SHA256(payload + requestId, webhookSecret)
            var dataToSign = payload;
            if (!string.IsNullOrEmpty(requestId))
            {
                dataToSign += requestId;
            }

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(webhookSecret));
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataToSign));
            var computedHashString = BitConverter.ToString(computedHash)
                .Replace("-", "", StringComparison.Ordinal)
                .ToLowerInvariant();

            // Comparação segura contra timing attacks
            if (!TimingSafeEquals(receivedHash, computedHashString))
            {
                _logger.LogWarning("Mercado Pago signature hash mismatch");
                return false;
            }

            // Sanitize user-provided requestId before logging to prevent log forging
            var safeRequestId = requestId?.Replace("\r", string.Empty).Replace("\n", string.Empty);
            _logger.LogDebug("Mercado Pago webhook signature validated successfully. Request ID: {RequestId}", safeRequestId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error validating Mercado Pago signature");
            return false;
        }
    }

    /// <summary>
    /// Comparação segura contra timing attacks.
    /// </summary>
    private static bool TimingSafeEquals(string left, string right)
    {
        if (left == null || right == null)
            return false;

        if (left.Length != right.Length)
            return false;

        var result = 0;
        for (var i = 0; i < left.Length; i++)
        {
            result |= left[i] ^ right[i];
        }

        return result == 0;
    }
}
