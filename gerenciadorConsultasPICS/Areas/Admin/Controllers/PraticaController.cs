using gerenciadorConsultasPICS.Areas.Usuario.Controllers;
using gerenciadorConsultasPICS.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace gerenciadorConsultasPICS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PraticaController : Controller
    {
        private readonly ILogger<AgendamentoController> _logger;
        private readonly IPraticaRepository _praticaRepository;

        public PraticaController(
            ILogger<AgendamentoController> logger,
            IPraticaRepository praticaRepository)
        {
            _logger = logger;
            _praticaRepository = praticaRepository;
        }

        [HttpGet]
        public async Task<JsonResult> ObterPraticasPorInstituicao(int idInstituicao)
        {
            var praticas = await _praticaRepository.ObterPorInstituicao(idInstituicao);
            if (praticas.Any())
                return Json(new { sucesso = true, listaPraticas = praticas });
            else
                return Json(new { sucesso = false, mensagem = "Nenhuma prática encontrada para a instituição informada." });
        }
    }
}
