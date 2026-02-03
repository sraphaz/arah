using Araponga.Api;
using Araponga.Api.Contracts.Media;
using Araponga.Api.Security;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Media;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Araponga.Api.Controllers;

/// <summary>
/// Controller para operações com mídias (imagens, vídeos, etc.).
/// </summary>
[ApiController]
[Route("api/v1/media")]
[Produces("application/json")]
[Tags("Media")]
public sealed class MediaController : ControllerBase
{
    private readonly MediaService _mediaService;
    private readonly CurrentUserAccessor _currentUserAccessor;

    public MediaController(
        MediaService mediaService,
        CurrentUserAccessor currentUserAccessor)
    {
        _mediaService = mediaService;
        _currentUserAccessor = currentUserAccessor;
    }

    /// <summary>
    /// Faz upload de uma mídia (imagem, vídeo, etc.).
    /// </summary>
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(MediaAssetResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<MediaAssetResponse>> UploadMedia(
        [FromForm] IFormFile file,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        if (file == null || file.Length == 0)
        {
            return BadRequest(new { error = "Arquivo é obrigatório." });
        }

        if (string.IsNullOrWhiteSpace(file.ContentType))
        {
            return BadRequest(new { error = "Tipo MIME do arquivo é obrigatório." });
        }

        using var stream = file.OpenReadStream();
        var result = await _mediaService.UploadMediaAsync(
            stream,
            file.ContentType,
            file.FileName,
            userContext.User.Id,
            cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error });
        }

        var mediaAsset = result.Value!;
        var response = new MediaAssetResponse(
            mediaAsset.Id,
            mediaAsset.UploadedByUserId,
            mediaAsset.MediaType.ToString().ToUpperInvariant(),
            mediaAsset.MimeType,
            mediaAsset.StorageKey,
            mediaAsset.SizeBytes,
            mediaAsset.WidthPx,
            mediaAsset.HeightPx,
            mediaAsset.Checksum,
            mediaAsset.CreatedAtUtc,
            mediaAsset.IsDeleted);

        return Created($"/api/v1/media/{mediaAsset.Id}", response);
    }

    /// <summary>
    /// Faz download de uma mídia.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadMedia(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var mediaAsset = await _mediaService.GetMediaAssetAsync(id, cancellationToken);
        if (mediaAsset is null || mediaAsset.IsDeleted)
        {
            return NotFound(new { error = "Mídia não encontrada." });
        }

        var urlResult = await _mediaService.GetMediaUrlAsync(id, null, cancellationToken);
        if (urlResult.IsFailure)
        {
            return NotFound(new { error = urlResult.Error });
        }

        // Para armazenamento local, retornar o arquivo diretamente
        // Em produção com cloud storage, isso retornaria uma redirect ou signed URL
        var url = urlResult.Value!;
        if (url.StartsWith("/media/", StringComparison.OrdinalIgnoreCase))
        {
            // URL relativa para armazenamento local - retornar arquivo
            var storageKey = url.Substring(7); // Remove "/media/"
            return File($"/media/{storageKey}", mediaAsset.MimeType, enableRangeProcessing: true);
        }

        // Para cloud storage, retornar redirect
        return Redirect(url);
    }

    /// <summary>
    /// Obtém informações sobre uma mídia.
    /// </summary>
    [HttpGet("{id:guid}/info")]
    [ProducesResponseType(typeof(MediaAssetResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MediaAssetResponse>> GetMediaInfo(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var mediaAsset = await _mediaService.GetMediaAssetAsync(id, cancellationToken);
        if (mediaAsset is null)
        {
            return NotFound(new { error = "Mídia não encontrada." });
        }

        var response = new MediaAssetResponse(
            mediaAsset.Id,
            mediaAsset.UploadedByUserId,
            mediaAsset.MediaType.ToString().ToUpperInvariant(),
            mediaAsset.MimeType,
            mediaAsset.StorageKey,
            mediaAsset.SizeBytes,
            mediaAsset.WidthPx,
            mediaAsset.HeightPx,
            mediaAsset.Checksum,
            mediaAsset.CreatedAtUtc,
            mediaAsset.IsDeleted);

        return Ok(response);
    }

    /// <summary>
    /// Deleta uma mídia.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMedia(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _mediaService.DeleteMediaAsync(id, userContext.User.Id, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error });
        }

        return NoContent();
    }
}