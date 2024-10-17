using gerenciadorConsultasPICS.Areas.Usuario.Controllers;
using gerenciadorConsultasPICS.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace gerenciadorConsultasPICS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EstadoController : Controller
    {
        private readonly ILogger<AgendamentoController> _logger;
        private readonly IEstadoRepository _estadoRepository;

        public EstadoController(
            ILogger<AgendamentoController> logger,
            IEstadoRepository estadoRepository)
        {
            _logger = logger;
            _estadoRepository = estadoRepository;
        }

        [HttpGet]
        public async Task<JsonResult> ObterEstados()
        {
            var estados = await _estadoRepository.ObterTodosAsync();
            if (estados.Any())
                return Json(new { sucesso = true, listaEstados = estados });
            else
                return Json(new { sucesso = false, mensagem = "Nenhum estado encontrado." });
        }
    }
}
