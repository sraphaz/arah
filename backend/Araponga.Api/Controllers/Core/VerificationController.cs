using Araponga.Api.Contracts.Verification;
using Araponga.Api.Security;
using Araponga.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/verification")]
[Produces("application/json")]
[Tags("Verification")]
public sealed class VerificationController : ControllerBase
{
    private readonly CurrentUserAccessor _currentUserAccessor;
    private readonly VerificationQueueService _verificationQueue;

    public VerificationController(
        CurrentUserAccessor currentUserAccessor,
        VerificationQueueService verificationQueue)
    {
        _currentUserAccessor = currentUserAccessor;
        _verificationQueue = verificationQueue;
    }

    /// <summary>
    /// Submete documento para verificação de identidade global (fallback humano: SystemAdmin).
    /// </summary>
    [HttpPost("identity/document")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SubmitIdentityDocument(
        [FromBody] SubmitIdentityDocumentRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _verificationQueue.SubmitIdentityDocumentAsync(
            userContext.User.Id,
            request.EvidenceId,
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            return BadRequest(new { error = result.Error ?? "Unable to submit identity verification." });
        }

        return Ok(new { message = "Identity verification submitted.", workItemId = result.Value.Id });
    }
}

