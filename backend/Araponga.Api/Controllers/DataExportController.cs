using Araponga.Api.Security;
using Araponga.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Text;

namespace Araponga.Api.Controllers;

/// <summary>
/// Controller para exportação e exclusão de dados do usuário (LGPD).
/// </summary>
[ApiController]
[Route("api/v1/users/me")]
[Produces("application/json")]
[Tags("Data Export")]
public sealed class DataExportController : ControllerBase
{
    private readonly DataExportService _dataExportService;
    private readonly AccountDeletionService _accountDeletionService;
    private readonly CurrentUserAccessor _currentUserAccessor;

    public DataExportController(
        DataExportService dataExportService,
        AccountDeletionService accountDeletionService,
        CurrentUserAccessor currentUserAccessor)
    {
        _dataExportService = dataExportService;
        _accountDeletionService = accountDeletionService;
        _currentUserAccessor = currentUserAccessor;
    }

    /// <summary>
    /// Exporta todos os dados do usuário autenticado em formato JSON (LGPD).
    /// </summary>
    [HttpGet("export")]
    [EnableRateLimiting("read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> ExportData(
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _dataExportService.ExportUserDataAsync(
            userContext.User.Id,
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            return BadRequest(new { error = result.Error ?? "Unable to export user data." });
        }

        var json = _dataExportService.SerializeToJson(result.Value);
        var bytes = Encoding.UTF8.GetBytes(json);

        return File(
            bytes,
            "application/json",
            $"user-data-export-{userContext.User.Id}-{DateTime.UtcNow:yyyyMMddHHmmss}.json");
    }

    /// <summary>
    /// Exclui a conta do usuário autenticado, anonimizando dados pessoais (LGPD).
    /// </summary>
    [HttpDelete]
    [EnableRateLimiting("write")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> DeleteAccount(
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        // Verificar se pode deletar
        var canDeleteResult = await _accountDeletionService.CanDeleteUserAsync(
            userContext.User.Id,
            cancellationToken);

        if (!canDeleteResult.IsSuccess || !canDeleteResult.Value)
        {
            return BadRequest(new { error = canDeleteResult.Error ?? "Account cannot be deleted at this time." });
        }

        // Anonimizar dados
        var anonymizeResult = await _accountDeletionService.AnonymizeUserDataAsync(
            userContext.User.Id,
            cancellationToken);

        if (!anonymizeResult.IsSuccess)
        {
            return BadRequest(new { error = anonymizeResult.Error ?? "Unable to delete account." });
        }

        return Ok(new { message = "Account deleted successfully. All personal data has been anonymized." });
    }
}
