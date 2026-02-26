using AgendaPics.Application.Common.Mediator;
using AgendaPics.Application.Features.Localidades.Queries;
using Microsoft.AspNetCore.Mvc;

namespace AgendaPics.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class CidadeController : Controller
{
    private readonly IMediator _mediator;

    public CidadeController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> ObterCidades(short idEstado)
    {
        var query = new ObterCidadesQuery(idEstado);
        var result = await _mediator.Send(query);

        if (result.IsFailure)
            return Json(new { sucesso = false, mensagem = result.Error });

        return Json(new { sucesso = true, listaCidades = result.Value });
    }
}
