using AgendaPics.Application.Common.Mediator;
using Microsoft.AspNetCore.Mvc;

namespace AgendaPics.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class EstadoController : Controller
{
    private readonly IMediator _mediator;

    public EstadoController(IMediator mediator)
    {
        _mediator = mediator;
    }
}
