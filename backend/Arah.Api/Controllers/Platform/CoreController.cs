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
    private const string InstanceTokenHeader = "X-Arah-Instance-Token";

    private readonly CoreInstanceService _instances;
    private readonly CoreDirectoryService _directory;
    private readonly CoreReleaseService _releases;

    public CoreController(
        CoreInstanceService instances,
        CoreDirectoryService directory,
        CoreReleaseService releases)
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
            registration.PrivateKeyPem,
            registration.InstanceAuthToken);
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

    [AllowAnonymous]
    [HttpPost("instances/{id:guid}/heartbeat")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Heartbeat(
        Guid id,
        [FromBody] CoreHeartbeatRequest request,
        [FromHeader(Name = InstanceTokenHeader)] string? instanceToken)
    {
        if (_instances.GetById(id) is null)
        {
            return NotFound();
        }

        if (!IsAuthorizedForInstance(id, instanceToken))
        {
            return Unauthorized();
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
        var list = _releases.ListByChannel(channel).Select(ToReleaseResponse);
        return Ok(list);
    }

    [HttpPost("releases")]
    [ProducesResponseType(typeof(CoreReleaseResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<CoreReleaseResponse> PublishRelease([FromBody] PublishCoreReleaseRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Version) || string.IsNullOrWhiteSpace(request.Channel))
        {
            return BadRequest("version and channel are required");
        }

        if (request.SchemaVersion < 1)
        {
            return BadRequest("schemaVersion must be >= 1");
        }

        var release = _releases.Publish(request.Version, request.Channel, request.SchemaVersion);
        return Created($"api/v1/core/releases", ToReleaseResponse(release));
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
            return Created("api/v1/core/directory/territories", ToDirectoryResponse(entry));
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

    private bool IsAuthorizedForInstance(Guid instanceId, string? instanceToken)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return true;
        }

        return _instances.ValidateAuthToken(instanceId, instanceToken);
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

    private static CoreReleaseResponse ToReleaseResponse(CoreRelease release) =>
        new(release.Id, release.Version, release.Channel, release.SchemaVersion, release.PublishedAtUtc);

    private static DirectoryTerritoryResponse ToDirectoryResponse(DirectoryTerritoryEntry entry) =>
        new(entry.TerritoryId, entry.InstanceId, entry.Name, entry.PublishedAtUtc);
}
