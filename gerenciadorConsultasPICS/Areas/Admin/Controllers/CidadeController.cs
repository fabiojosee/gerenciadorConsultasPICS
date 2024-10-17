using gerenciadorConsultasPICS.Areas.Usuario.Controllers;
using gerenciadorConsultasPICS.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace gerenciadorConsultasPICS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CidadeController : Controller
    {
        private readonly ILogger<AgendamentoController> _logger;
        private readonly ICidadeRepository _cidadeRepository;

        public CidadeController(
            ILogger<AgendamentoController> logger,
            ICidadeRepository cidadeRepository)
        {
            _logger = logger;
            _cidadeRepository = cidadeRepository;
        }

        [HttpGet]
        public async Task<JsonResult> ObterCidades(short idEstado)
        {
            var cidades = await _cidadeRepository.ObterPorEstado(idEstado);
            if (cidades.Any())
                return Json(new { sucesso = true, listaCidades = cidades });
            else
                return Json(new { sucesso = false, mensagem = "Nenhuma cidade encontrada para o estado informado." });
        }
    }
}
