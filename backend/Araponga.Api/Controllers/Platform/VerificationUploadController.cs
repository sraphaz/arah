using Araponga.Api.Security;
using Araponga.Api.Contracts.Common;
using Araponga.Application.Services;
using Araponga.Modules.Moderation.Domain.Evidence;
using Microsoft.AspNetCore.Mvc;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/verification")]
[Produces("application/json")]
[Tags("Verification")]
public sealed class VerificationUploadController : ControllerBase
{
    private readonly CurrentUserAccessor _currentUserAccessor;
    private readonly DocumentEvidenceService _evidenceService;
    private readonly VerificationQueueService _verificationQueue;

    public VerificationUploadController(
        CurrentUserAccessor currentUserAccessor,
        DocumentEvidenceService evidenceService,
        VerificationQueueService verificationQueue)
    {
        _currentUserAccessor = currentUserAccessor;
        _evidenceService = evidenceService;
        _verificationQueue = verificationQueue;
    }

    /// <summary>
    /// Upload de documento para verificação de identidade global.
    /// </summary>
    [HttpPost("identity/document/upload")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UploadIdentityDocument(
        [FromForm] FileUploadRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var file = request.File;
        if (file is null || file.Length <= 0)
        {
            return BadRequest(new { error = "file is required." });
        }

        await using var stream = file.OpenReadStream();
        var created = await _evidenceService.CreateAsync(
            userContext.User.Id,
            territoryId: null,
            DocumentEvidenceKind.Identity,
            file.FileName,
            file.ContentType,
            stream,
            cancellationToken);

        if (!created.IsSuccess || created.Value is null)
        {
            return BadRequest(new { error = created.Error ?? "Unable to upload evidence." });
        }

        // Enfileirar usando evidenceId no payload (por enquanto ainda via documentRef string)
        var work = await _verificationQueue.SubmitIdentityDocumentAsync(
            userContext.User.Id,
            created.Value.Id,
            cancellationToken);

        if (!work.IsSuccess || work.Value is null)
        {
            return BadRequest(new { error = work.Error ?? "Unable to submit identity verification." });
        }

        return Ok(new { message = "Identity verification submitted.", evidenceId = created.Value.Id, workItemId = work.Value.Id });
    }
}

