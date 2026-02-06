using Araponga.Api.Security;
using Araponga.Application.Services;
using Araponga.Domain.Territories;
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
    private readonly TerritoryService _territoryService;
    private readonly ILogger<AdminSeedController> _logger;

    public AdminSeedController(
        SubscriptionPlanSeedService seedService,
        TerritoryService territoryService,
        ILogger<AdminSeedController> logger)
    {
        _seedService = seedService;
        _territoryService = territoryService;
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

    /// <summary>
    /// Cadastra o território Camburi (São Sebastião, SP) como território padrão, se ainda não existir.
    /// Perímetro: polígono com os 9 vértices da região; centroide (-23.76281, -45.63691).
    /// </summary>
    [HttpPost("territories/camburi")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SeedTerritoryCamburi(CancellationToken cancellationToken)
    {
        const string name = "Camburi";
        const string city = "São Sebastião";
        const string state = "SP";
        const double latitude = -23.76281;
        const double longitude = -45.63691;
        const double radiusKm = 3.5;
        const string description = "Praia e bairro de Camburi, São Sebastião, SP. Perímetro definido pelo polígono da região.";

        var boundaryPolygon = new List<TerritoryBoundaryPoint>
        {
            new(-23.77712, -45.66050),
            new(-23.76220, -45.66387),
            new(-23.74968, -45.66118),
            new(-23.73371, -45.62439),
            new(-23.75068, -45.60883),
            new(-23.76304, -45.60608),
            new(-23.77456, -45.62988),
            new(-23.78168, -45.63744),
            new(-23.78252, -45.65002),
        };

        try
        {
            var existing = await _territoryService.SearchAsync(name, city, state, cancellationToken);
            var alreadyExists = existing.Any(t =>
                string.Equals(t.Name, name, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(t.City, city, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(t.State, state, StringComparison.OrdinalIgnoreCase));

            if (alreadyExists)
            {
                _logger.LogInformation("Território Camburi (São Sebastião, SP) já existe. Nenhuma alteração.");
                return Ok(new { message = "Territory Camburi already exists.", territoryId = existing.First(t =>
                    string.Equals(t.Name, name, StringComparison.OrdinalIgnoreCase)).Id });
            }

            var result = await _territoryService.CreateAsync(
                name,
                description,
                city,
                state,
                latitude,
                longitude,
                cancellationToken,
                radiusKm,
                boundaryPolygon,
                TerritoryStatus.Active);

            if (!result.Success || result.Territory is null)
            {
                _logger.LogWarning("Falha ao criar território Camburi: {Error}", result.Error);
                return StatusCode(500, new { error = result.Error ?? "Failed to create territory Camburi." });
            }

            _logger.LogInformation("Território Camburi (São Sebastião, SP) criado como padrão. Id: {TerritoryId}", result.Territory.Id);
            return StatusCode(StatusCodes.Status201Created, new
            {
                message = "Territory Camburi (São Sebastião, SP) created as default.",
                territoryId = result.Territory.Id,
                name = result.Territory.Name,
                city = result.Territory.City,
                state = result.Territory.State,
                latitude = result.Territory.Latitude,
                longitude = result.Territory.Longitude,
                radiusKm = result.Territory.RadiusKm,
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding territory Camburi");
            return StatusCode(500, new { error = "Error seeding territory Camburi", message = ex.Message });
        }
    }

}
