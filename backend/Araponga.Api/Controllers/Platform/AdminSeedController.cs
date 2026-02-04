using Araponga.Api.Security;
using Araponga.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Araponga.Api.Controllers;

/// <summary>
/// Controller administrativo para executar seeds e inicializações do sistema.
/// </summary>
[ApiController]
[Route("api/v1/admin/seed")]
[Produces("application/json")]
[Tags("Admin - Seed")]
[Authorize(Policy = "SystemAdmin")]
public sealed class AdminSeedController : ControllerBase
{
    private readonly SubscriptionPlanSeedService _seedService;
    private readonly ILogger<AdminSeedController> _logger;

    public AdminSeedController(
        SubscriptionPlanSeedService seedService,
        ILogger<AdminSeedController> logger)
    {
        _seedService = seedService;
        _logger = logger;
    }

    /// <summary>
    /// Cria o plano FREE padrão se não existir.
    /// </summary>
    [HttpPost("subscription-plans/free")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SeedFreePlan(CancellationToken cancellationToken)
    {
        try
        {
            await _seedService.SeedDefaultFreePlanAsync(cancellationToken);
            return Ok(new { message = "FREE plan seeded successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding FREE plan");
            return StatusCode(500, new { error = "Error seeding FREE plan", message = ex.Message });
        }
    }
}
