using Arah.Api.Contracts.Core;
using Arah.Core.Application;
using Arah.Core.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Arah.Api.Controllers.Platform;

[ApiController]
[Route("api/v1/core")]
[Authorize(Policy = "SystemAdmin")]
[Produces("application/json")]
[Tags("Arah Core")]
public sealed class CoreController : ControllerBase
{
    private readonly CoreInstanceService _instances;
    private readonly ICoreReleaseCatalog _releases;

    public CoreController(CoreInstanceService instances, ICoreReleaseCatalog releases)
    {
        _instances = instances;
        _releases = releases;
    }

    [HttpPost("instances")]
    [ProducesResponseType(typeof(CoreInstanceResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<CoreInstanceResponse> RegisterInstance([FromBody] RegisterCoreInstanceRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Mode) ||
            string.IsNullOrWhiteSpace(request.BaseUrl) ||
            string.IsNullOrWhiteSpace(request.Version))
        {
            return BadRequest("mode, baseUrl and version are required");
        }

        if (!Uri.TryCreate(request.BaseUrl, UriKind.Absolute, out var baseUrl))
        {
            return BadRequest("baseUrl must be an absolute URI");
        }

        var instance = _instances.Register(request.Mode, baseUrl, request.Version);
        var response = ToResponse(instance);
        return CreatedAtAction(nameof(GetInstance), new { id = instance.Id }, response);
    }

    [HttpGet("instances")]
    [ProducesResponseType(typeof(IEnumerable<CoreInstanceResponse>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<CoreInstanceResponse>> ListInstances()
    {
        var list = _instances.ListAll().Select(ToResponse);
        return Ok(list);
    }

    [HttpGet("instances/{id:guid}")]
    [ProducesResponseType(typeof(CoreInstanceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<CoreInstanceResponse> GetInstance(Guid id)
    {
        var instance = _instances.GetById(id);
        return instance is null ? NotFound() : Ok(ToResponse(instance));
    }

    [HttpPost("instances/{id:guid}/heartbeat")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Heartbeat(Guid id, [FromBody] CoreHeartbeatRequest request)
    {
        if (_instances.GetById(id) is null)
        {
            return NotFound();
        }

        var services = request.Services ?? new Dictionary<string, string>();
        var uptime = TimeSpan.FromSeconds(Math.Max(0, request.UptimeSeconds));
        _instances.RecordHeartbeat(id, services, uptime);
        return NoContent();
    }

    [HttpGet("releases")]
    [ProducesResponseType(typeof(IEnumerable<CoreReleaseResponse>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<CoreReleaseResponse>> ListReleases([FromQuery] string channel = "stable")
    {
        var list = _releases.ListByChannel(channel)
            .Select(r => new CoreReleaseResponse(
                r.Id,
                r.Version,
                r.Channel,
                r.SchemaVersion,
                r.PublishedAtUtc));
        return Ok(list);
    }

    private static CoreInstanceResponse ToResponse(CoreInstance instance) =>
        new(
            instance.Id,
            instance.Mode,
            instance.BaseUrl.ToString(),
            instance.Version,
            instance.Status.ToString(),
            instance.RegisteredAtUtc,
            instance.LastHeartbeatUtc,
            instance.LastServices,
            instance.LastUptime.HasValue ? (long)instance.LastUptime.Value.TotalSeconds : null);
}
