using Araponga.Api.Security;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Application.Models;
using Araponga.Domain.Membership;
using Araponga.Modules.Moderation.Application.Interfaces;
using Araponga.Modules.Moderation.Domain.Evidence;
using Microsoft.AspNetCore.Mvc;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/territories/{territoryId:guid}/evidences")]
[Produces("application/json")]
[Tags("Admin")]
public sealed class TerritoryEvidencesController : ControllerBase
{
    private readonly CurrentUserAccessor _currentUserAccessor;
    private readonly AccessEvaluator _accessEvaluator;
    private readonly IDocumentEvidenceRepository _evidenceRepository;
    private readonly IFileStorage _storage;
    private readonly IAuditLogger _auditLogger;

    public TerritoryEvidencesController(
        CurrentUserAccessor currentUserAccessor,
        AccessEvaluator accessEvaluator,
        IDocumentEvidenceRepository evidenceRepository,
        IFileStorage storage,
        IAuditLogger auditLogger)
    {
        _currentUserAccessor = currentUserAccessor;
        _accessEvaluator = accessEvaluator;
        _evidenceRepository = evidenceRepository;
        _storage = storage;
        _auditLogger = auditLogger;
    }

    /// <summary>
    /// Download por proxy de uma evidência territorial (Curator ou Moderator).
    /// Atualmente voltado para evidências de residência.
    /// </summary>
    [HttpGet("{evidenceId:guid}/download")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Download(
        [FromRoute] Guid territoryId,
        [FromRoute] Guid evidenceId,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var canAccess =
            await _accessEvaluator.HasCapabilityAsync(userContext.User.Id, territoryId, MembershipCapabilityType.Curator, cancellationToken) ||
            await _accessEvaluator.HasCapabilityAsync(userContext.User.Id, territoryId, MembershipCapabilityType.Moderator, cancellationToken);
        if (!canAccess)
        {
            return Unauthorized();
        }

        var evidence = await _evidenceRepository.GetByIdAsync(evidenceId, cancellationToken);
        if (evidence is null)
        {
            return NotFound();
        }

        if (evidence.Kind != DocumentEvidenceKind.Residency || evidence.TerritoryId != territoryId)
        {
            return NotFound();
        }

        if (evidence.StorageProvider != _storage.Provider)
        {
            return Conflict(new { error = "Evidence storage provider mismatch." });
        }

        var now = DateTime.UtcNow;
        await _auditLogger.LogAsync(
            new AuditEntry("document_evidence.downloaded", userContext.User.Id, territoryId, evidence.Id, now),
            cancellationToken);

        var stream = await _storage.OpenReadAsync(evidence.StorageKey, cancellationToken);
        var downloadName = evidence.OriginalFileName ?? $"evidence-{evidence.Id:N}";

        return File(stream, evidence.ContentType, downloadName);
    }
}

