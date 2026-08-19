using Arah.Api.Contracts.Fiscal;
using Arah.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Arah.Api.Controllers.Fiscal;

[ApiController]
[Route("api/v1/fiscal-packs")]
[Produces("application/json")]
[Tags("Fiscal Packs")]
public sealed class FiscalPacksController : ControllerBase
{
    private readonly IFiscalPackCatalog _catalog;

    public FiscalPacksController(IFiscalPackCatalog catalog)
    {
        _catalog = catalog;
    }

    /// <summary>
    /// Lista pacotes fiscais disponíveis na instância (MVP: brazil.v1).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<FiscalPackResponse>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyList<FiscalPackResponse>> List()
    {
        var packs = _catalog.ListPacks()
            .Select(p => new FiscalPackResponse(p.Id, p.CountryCode, p.DisplayName, p.Capabilities))
            .ToArray();
        return Ok(packs);
    }
}
