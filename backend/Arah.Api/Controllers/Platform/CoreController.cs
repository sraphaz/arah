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
    private readonly CoreDirectoryService _directory;
    private readonly ICoreReleaseCatalog _releases;

    public CoreController(
        CoreInstanceService instances,
        CoreDirectoryService directory,
        ICoreReleaseCatalog releases)
    {
        _instances = instances;
        _directory = directory;
        _releases = releases;
    }

    [HttpPost("instances")]
    [ProducesResponseType(typeof(RegisterCoreInstanceResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<RegisterCoreInstanceResponse> RegisterInstance([FromBody] RegisterCoreInstanceRequest request)
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

        var registration = _instances.Register(request.Mode, baseUrl, request.Version);
        var response = new RegisterCoreInstanceResponse(
            ToResponse(registration.Instance),
            registration.PrivateKeyPem);
        return CreatedAtAction(nameof(GetInstance), new { id = registration.Instance.Id }, response);
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

    [HttpPost("directory/territories")]
    [ProducesResponseType(typeof(DirectoryTerritoryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<DirectoryTerritoryResponse> PublishTerritory([FromBody] PublishDirectoryTerritoryRequest request)
    {
        if (request.TerritoryId == Guid.Empty || request.InstanceId == Guid.Empty)
        {
            return BadRequest("territoryId and instanceId are required");
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("name is required");
        }

        try
        {
            var entry = _directory.PublishTerritory(request.TerritoryId, request.InstanceId, request.Name);
            return Created(
                $"api/v1/core/directory/territories",
                ToDirectoryResponse(entry));
        }
        catch (InvalidOperationException)
        {
            return NotFound("instance is not registered");
        }
    }

    [HttpGet("directory/territories")]
    [ProducesResponseType(typeof(IEnumerable<DirectoryTerritoryResponse>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<DirectoryTerritoryResponse>> ListDirectoryTerritories()
    {
        var list = _directory.ListTerritories().Select(ToDirectoryResponse);
        return Ok(list);
    }

    private static CoreInstanceResponse ToResponse(CoreInstance instance) =>
        new(
            instance.Id,
            instance.Mode,
            instance.BaseUrl.ToString(),
            instance.Version,
            instance.PublicKeyPem,
            instance.Status.ToString(),
            instance.RegisteredAtUtc,
            instance.LastHeartbeatUtc,
            instance.LastServices,
            instance.LastUptime.HasValue ? (long)instance.LastUptime.Value.TotalSeconds : null);

    private static DirectoryTerritoryResponse ToDirectoryResponse(DirectoryTerritoryEntry entry) =>
        new(entry.TerritoryId, entry.InstanceId, entry.Name, entry.PublishedAtUtc);
}
