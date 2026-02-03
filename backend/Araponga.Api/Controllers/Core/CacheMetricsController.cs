using Araponga.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Araponga.Api.Controllers;

/// <summary>
/// Controller para expor métricas de cache (hit/miss rates).
/// </summary>
[ApiController]
[Route("api/v1/admin/cache-metrics")]
[Authorize] // Requer autenticação - em produção, adicionar role SystemAdmin
public sealed class CacheMetricsController : ControllerBase
{
    private readonly CacheMetricsService _metricsService;

    public CacheMetricsController(CacheMetricsService metricsService)
    {
        _metricsService = metricsService;
    }

    /// <summary>
    /// Obtém as métricas atuais de cache.
    /// </summary>
    [HttpGet]
    public ActionResult<CacheMetrics> GetMetrics()
    {
        var metrics = _metricsService.GetMetrics();
        return Ok(metrics);
    }

    /// <summary>
    /// Reseta as métricas de cache.
    /// </summary>
    [HttpPost("reset")]
    public IActionResult ResetMetrics()
    {
        _metricsService.Reset();
        return NoContent();
    }
}
