using Araponga.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Text;
using System.Text.Json;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/webhooks/stripe")]
[Produces("application/json")]
[Tags("Webhooks")]
public sealed class StripeWebhookController : ControllerBase
{
    private readonly StripeWebhookService _webhookService;
    private readonly ILogger<StripeWebhookController> _logger;
    private readonly IConfiguration _configuration;

    public StripeWebhookController(
        StripeWebhookService webhookService,
        ILogger<StripeWebhookController> logger,
        IConfiguration configuration)
    {
        _webhookService = webhookService;
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Recebe webhooks do Stripe para eventos de assinaturas.
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

            // Validar assinatura do Stripe
            var stripeSignature = Request.Headers["Stripe-Signature"].FirstOrDefault();
            if (!ValidateStripeSignature(body, stripeSignature))
            {
                _logger.LogWarning("Invalid Stripe webhook signature");
                return BadRequest("Invalid signature");
            }
            
            _logger.LogInformation("Received Stripe webhook");

            // Parse do JSON do evento
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;

            // Extrair tipo do evento
            var eventType = root.TryGetProperty("type", out var typeProp)
                ? typeProp.GetString()
                : null;

            if (string.IsNullOrEmpty(eventType))
            {
                _logger.LogWarning("Stripe webhook received without event type");
                return BadRequest("Event type is required");
            }

            // Extrair dados do evento
            var eventData = root.TryGetProperty("data", out var dataProp)
                ? dataProp.TryGetProperty("object", out var objectProp) ? objectProp : dataProp
                : root;

            // Processar evento
            var result = await _webhookService.ProcessEventAsync(
                eventType,
                eventData,
                cancellationToken);

            if (result.IsFailure)
            {
                _logger.LogError("Error processing Stripe webhook: {Error}", result.Error);
                return BadRequest(new { error = result.Error });
            }

            return Ok(new { received = true, eventType });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling Stripe webhook");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Valida a assinatura do webhook do Stripe.
    /// </summary>
    private bool ValidateStripeSignature(string payload, string? signature)
    {
        if (string.IsNullOrEmpty(signature))
        {
            _logger.LogWarning("Stripe webhook signature header is missing");
            return false;
        }

        var webhookSecret = _configuration["Stripe:WebhookSecret"];
        if (string.IsNullOrEmpty(webhookSecret))
        {
            // Em desenvolvimento, permitir sem validação se não houver secret configurado
            if (_configuration.GetValue<bool>("ASPNETCORE_ENVIRONMENT") == false || 
                _configuration["ASPNETCORE_ENVIRONMENT"]?.Equals("Development", StringComparison.OrdinalIgnoreCase) == true)
            {
                _logger.LogWarning("Stripe webhook secret not configured. Skipping validation in development.");
                return true;
            }
            
            _logger.LogError("Stripe webhook secret not configured in production");
            return false;
        }

        try
        {
            // Usar Stripe.net para validar assinatura
            var stripeEvent = EventUtility.ConstructEvent(
                payload,
                signature,
                webhookSecret,
                throwOnApiVersionMismatch: false);

            if (stripeEvent == null)
            {
                _logger.LogWarning("Failed to construct Stripe event from signature");
                return false;
            }

            _logger.LogDebug("Stripe webhook signature validated successfully. Event ID: {EventId}", stripeEvent.Id);
            return true;
        }
        catch (StripeException ex)
        {
            _logger.LogWarning(ex, "Stripe signature validation failed: {Message}", ex.Message);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error validating Stripe signature");
            return false;
        }
    }
}
