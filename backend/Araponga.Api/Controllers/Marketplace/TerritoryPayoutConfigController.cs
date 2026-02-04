using Araponga.Api;
using Araponga.Api.Contracts.Payout;
using Araponga.Api.Security;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Modules.Marketplace.Domain;
using Araponga.Domain.Users;
using Microsoft.AspNetCore.Mvc;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/territories/{territoryId:guid}/payout-config")]
[Produces("application/json")]
[Tags("Payout Configuration")]
public sealed class TerritoryPayoutConfigController : ControllerBase
{
    private readonly TerritoryPayoutConfigService _configService;
    private readonly CurrentUserAccessor _currentUserAccessor;
    private readonly AccessEvaluator _accessEvaluator;

    public TerritoryPayoutConfigController(
        TerritoryPayoutConfigService configService,
        CurrentUserAccessor currentUserAccessor,
        AccessEvaluator accessEvaluator)
    {
        _configService = configService;
        _currentUserAccessor = currentUserAccessor;
        _accessEvaluator = accessEvaluator;
    }

    /// <summary>
    /// Obtém a configuração ativa de payout para um território.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(TerritoryPayoutConfigResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TerritoryPayoutConfigResponse>> GetActiveConfig(
        [FromRoute] Guid territoryId,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        // Verificar se usuário tem permissão para ver configurações (Admin, Curator, ou FinancialManager)
        // Por enquanto, apenas verificamos se é admin do sistema
        // TODO: Adicionar verificação de capability específica quando o sistema de capabilities estiver completo
        var isAdmin = await _accessEvaluator.HasSystemPermissionAsync(
            userContext.User.Id,
            SystemPermissionType.SystemAdmin,
            cancellationToken);

        if (!isAdmin)
        {
            // TODO: Verificar se é residente do território com papel apropriado (FinancialManager)
            return Forbid();
        }

        var config = await _configService.GetActiveConfigAsync(territoryId, cancellationToken);
        if (config is null)
        {
            return NotFound(new { error = "Payout configuration not found." });
        }

        var response = new TerritoryPayoutConfigResponse(
            config.Id,
            config.TerritoryId,
            config.RetentionPeriodDays,
            config.MinimumPayoutAmountInCents,
            config.MaximumPayoutAmountInCents,
            config.Frequency.ToString(),
            config.AutoPayoutEnabled,
            config.RequiresApproval,
            config.Currency,
            config.IsActive,
            config.CreatedAtUtc,
            config.UpdatedAtUtc);

        return Ok(response);
    }

    /// <summary>
    /// Cria ou atualiza a configuração de payout para um território.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TerritoryPayoutConfigResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<TerritoryPayoutConfigResponse>> UpsertConfig(
        [FromRoute] Guid territoryId,
        [FromBody] TerritoryPayoutConfigRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        // Verificar se usuário tem permissão para gerenciar configurações
        // Por enquanto, apenas verificamos se é admin do sistema
        // TODO: Adicionar verificação de capability específica quando o sistema de capabilities estiver completo
        var isAdmin = await _accessEvaluator.HasSystemPermissionAsync(
            userContext.User.Id,
            SystemPermissionType.SystemAdmin,
            cancellationToken);

        if (!isAdmin)
        {
            // TODO: Verificar se é residente do território com papel apropriado (FinancialManager)
            return Forbid();
        }

        // Validar e converter Frequency
        if (!Enum.TryParse<PayoutFrequency>(request.Frequency, ignoreCase: true, out var frequency))
        {
            return BadRequest(new { error = $"Invalid frequency. Must be one of: Daily, Weekly, Monthly, Manual" });
        }

        var result = await _configService.UpsertConfigAsync(
            territoryId,
            request.RetentionPeriodDays,
            request.MinimumPayoutAmountInCents,
            request.MaximumPayoutAmountInCents,
            frequency,
            request.AutoPayoutEnabled,
            request.RequiresApproval,
            request.Currency,
            userContext.User.Id,
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            return BadRequest(new { error = result.Error });
        }

        var config = result.Value;
        var response = new TerritoryPayoutConfigResponse(
            config.Id,
            config.TerritoryId,
            config.RetentionPeriodDays,
            config.MinimumPayoutAmountInCents,
            config.MaximumPayoutAmountInCents,
            config.Frequency.ToString(),
            config.AutoPayoutEnabled,
            config.RequiresApproval,
            config.Currency,
            config.IsActive,
            config.CreatedAtUtc,
            config.UpdatedAtUtc);

        return Ok(response);
    }
}
