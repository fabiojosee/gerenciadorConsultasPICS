using gerenciadorConsultasPICS.Areas.Admin.Models;
using gerenciadorConsultasPICS.Areas.Admin.ViewModels.Pratica;
using gerenciadorConsultasPICS.Areas.Usuario.Controllers;
using gerenciadorConsultasPICS.Repositories.Interfaces;
using gerenciadorConsultasPICS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace gerenciadorConsultasPICS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PraticaController : Controller
    {
        private readonly ILogger<AgendamentoController> _logger;
        private readonly IPraticaRepository _praticaRepository;
        private readonly IPraticaInstituicaoRepository _praticaInstituicaoRepository;
        private readonly IInstituicaoRepository _instituicaoRepository;
        private readonly IAgendamentoRepository _agendamentoRepository;
        private readonly ITokenService _tokenService;

        public PraticaController(
            ILogger<AgendamentoController> logger,
            IPraticaRepository praticaRepository,
            IPraticaInstituicaoRepository praticaInstituicaoRepository,
            IInstituicaoRepository instituicaoRepository,
            IAgendamentoRepository agendamentoRepository,
            ITokenService tokenService)
        {
            _logger = logger;
            _praticaRepository = praticaRepository;
            _praticaInstituicaoRepository = praticaInstituicaoRepository;
            _instituicaoRepository = instituicaoRepository;
            _agendamentoRepository = agendamentoRepository;
            _tokenService = tokenService;
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

        [HttpGet]
        [Authorize(Policy = "ApenasInstituicao")]
        public async Task<IActionResult> MinhasPraticas()
        {
            var usuarioInfo = _tokenService.ObterInformacoesToken();

            ViewBag.idInstituicao = usuarioInfo.idInstituicao.Value;

            var praticas = await _praticaInstituicaoRepository.ObterMinhasPraticas(usuarioInfo.idInstituicao.Value);

            return View(praticas);
        }

        [HttpGet]
        [Authorize(Policy = "ApenasAdmin")]
        public async Task<IActionResult> MinhasPraticasAdmin()
        {
            var praticas = await _praticaRepository.ObterTodosAsync();

            return View(praticas);
        }

        [HttpGet]
        [Authorize(Policy = "ApenasAdmin")]
        public IActionResult NovaPratica()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Policy = "ApenasAdmin")]
        public async Task<JsonResult> CriarPratica(PraticaViewModel praticaViewModel)
        {
            if (praticaViewModel is null)
                return Json(new { sucesso = false, mensagem = "Formato inválido." });

            if (praticaViewModel.nome is null)
                return Json(new { sucesso = false, mensagem = "O campo nome é obrigatório." });

            Pratica pratica = Pratica.PraticaFactory.CriarPratica(praticaViewModel.nome, praticaViewModel.descricao);
            await _praticaRepository.AdicionarAsync(pratica);

            return Json(new { sucesso = true });
        }

        [HttpGet]
        [Authorize(Policy = "ApenasAdmin")]
        public IActionResult NovaPraticaSucesso()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Policy = "ApenasAdmin")]
        public async Task<IActionResult> EdicaoPratica(short idPratica)
        {
            var pratica = await _praticaRepository.ObterPorIdAsync(idPratica);

            return View(new PraticaViewModel()
            {
                idPratica = pratica.idPratica,
                nome = pratica.nome,
                descricao = pratica.descricao
            });
        }

        [HttpPut]
        [Authorize(Policy = "ApenasAdmin")]
        public async Task<JsonResult> EditarPratica(PraticaViewModel praticaViewModel)
        {
            if (praticaViewModel is null)
                return Json(new { sucesso = false, mensagem = "Formato inválido." });

            if (praticaViewModel.nome is null)
                return Json(new { sucesso = false, mensagem = "O campo nome é obrigatório." });

            Pratica pratica = await _praticaRepository.ObterPorIdAsync(praticaViewModel.idPratica);
            if (pratica is null)
                return Json(new { sucesso = false, mensagem = "Prática não encontrada." });

            pratica.Atualizar(praticaViewModel.nome, praticaViewModel.descricao);
            await _praticaRepository.AtualizarAsync(pratica);

            return Json(new { sucesso = true });
        }

        [HttpGet]
        [Authorize(Policy = "ApenasAdmin")]
        public IActionResult EdicaoPraticaSucesso()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Policy = "ApenasInstituicao")]
        public async Task<IActionResult> VinculoPratica()
        {
            var usuarioInfo = _tokenService.ObterInformacoesToken();

            ViewBag.idInstituicao = usuarioInfo.idInstituicao.Value;
            ViewBag.Praticas = await _praticaRepository.ObterTodosAsync();

            return View();
        }

        [HttpGet]
        [Authorize(Policy = "ApenasInstituicao")]
        public async Task<IActionResult> EdicaoVinculoPratica(short idPratica)
        {
            var usuarioInfo = _tokenService.ObterInformacoesToken();
            ViewBag.idInstituicao = usuarioInfo.idInstituicao.Value;
            ViewBag.Praticas = await _praticaRepository.ObterTodosAsync();

            var praticaInstituicao = await _praticaInstituicaoRepository.ObterPorPraticaInstituicao(usuarioInfo.idInstituicao.Value, idPratica);

            return View(new VinculoPraticaViewModel
            {
                idPratica = praticaInstituicao.idPratica,
                idInstituicao = praticaInstituicao.idInstituicao,
                periodicidade = praticaInstituicao.periodicidade,
                qtdSessoes = praticaInstituicao.qtdSessoes,
                diaPermitidoParaAgendamento = praticaInstituicao.diaPermitidoParaAgendamento
            });
        }

        [HttpGet]
        [Authorize(Policy = "ApenasInstituicao")]
        public IActionResult EdicaoVinculoPraticaSucesso()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Policy = "ApenasAdmin")]
        public async Task<IActionResult> VinculoPraticaAdmin()
        {
            ViewBag.Instituicoes = await _instituicaoRepository.ObterTodosAsync();
            ViewBag.Praticas = await _praticaRepository.ObterTodosAsync();

            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<JsonResult> VincularPratica(VinculoPraticaViewModel vinculoPraticaViewModel)
        {
            if (vinculoPraticaViewModel is null)
                return Json(new { sucesso = false, mensagem = "Formato inválido." });

            if (vinculoPraticaViewModel.idInstituicao is null)
                return Json(new { sucesso = false, mensagem = "A seleção de instituição é obrigatória." });

            if (vinculoPraticaViewModel.idPratica is null)
                return Json(new { sucesso = false, mensagem = "A seleção de prática é obrigatória." });

            if (vinculoPraticaViewModel.periodicidade is null)
                return Json(new { sucesso = false, mensagem = "O campo periodicidade é obrigatório." });

            if (vinculoPraticaViewModel.qtdSessoes is null)
                return Json(new { sucesso = false, mensagem = "O campo quantidade de sessões é obrigatório." });

            if (vinculoPraticaViewModel.diaPermitidoParaAgendamento is null)
                return Json(new { sucesso = false, mensagem = "O campo dia da semana para os atendimentoss é obrigatório." });

            var praticaInstituicao = await _praticaInstituicaoRepository.ObterPorIdAsync([vinculoPraticaViewModel.idPratica.Value, vinculoPraticaViewModel.idInstituicao.Value]);
            if (praticaInstituicao is not null)
                return Json(new { sucesso = false, mensagem = "A prática selecionada já está vinculada à instituição." });

            PraticaInstituicao novoVinculo = PraticaInstituicao.PraticaInstituicaoFactory.CriarPraticaInstituicao(
                    vinculoPraticaViewModel.idPratica.Value,
                    vinculoPraticaViewModel.idInstituicao.Value,
                    vinculoPraticaViewModel.periodicidade.Value,
                    vinculoPraticaViewModel.qtdSessoes.Value,
                    vinculoPraticaViewModel.diaPermitidoParaAgendamento.Value
                );

            await _praticaInstituicaoRepository.AdicionarAsync(novoVinculo);

            return Json(new { sucesso = true });
        }

        [HttpPut]
        [Authorize]
        public async Task<JsonResult> EditarVinculoPratica(VinculoPraticaViewModel vinculoPraticaViewModel)
        {
            if (vinculoPraticaViewModel is null)
                return Json(new { sucesso = false, mensagem = "Formato inválido." });

            if (vinculoPraticaViewModel.idInstituicao is null)
                return Json(new { sucesso = false, mensagem = "A seleção de instituição é obrigatória." });

            if (vinculoPraticaViewModel.idPratica is null)
                return Json(new { sucesso = false, mensagem = "A seleção de prática é obrigatória." });

            if (vinculoPraticaViewModel.periodicidade is null)
                return Json(new { sucesso = false, mensagem = "O campo periodicidade é obrigatório." });

            if (vinculoPraticaViewModel.qtdSessoes is null)
                return Json(new { sucesso = false, mensagem = "O campo quantidade de sessões é obrigatório." });

            if (vinculoPraticaViewModel.diaPermitidoParaAgendamento is null)
                return Json(new { sucesso = false, mensagem = "O campo dia da semana para os atendimentoss é obrigatório." });

            var praticaInstituicao = await _praticaInstituicaoRepository.ObterPorIdAsync([vinculoPraticaViewModel.idPratica.Value, vinculoPraticaViewModel.idInstituicao.Value]);
            if (praticaInstituicao is null)
                return Json(new { sucesso = false, mensagem = "A prática não está vinculada à instituição." });

            praticaInstituicao.Atualizar(vinculoPraticaViewModel.periodicidade.Value, vinculoPraticaViewModel.qtdSessoes.Value, vinculoPraticaViewModel.diaPermitidoParaAgendamento.Value);

            await _praticaInstituicaoRepository.AtualizarAsync(praticaInstituicao);

            return Json(new { sucesso = true });
        }

        [HttpGet]
        [Authorize]
        public IActionResult VinculoPraticaSucesso()
        {
            var usuarioInfo = _tokenService.ObterInformacoesToken();
            ViewBag.idPerfil = usuarioInfo.idPerfil;

            return View();
        }

        [HttpDelete]
        [Authorize(Policy = "ApenasAdmin")]
        public async Task<JsonResult> ExcluirPratica(short idPratica)
        {
            var agendamentos = await _agendamentoRepository.ObterPorPratica(idPratica);
            if (agendamentos.Any())
                return Json(new { sucesso = false, mensagem = "A prática não pode ser excluída, pois já existem agendamentos associados a ela." });

            var vinculosPratica = await _praticaInstituicaoRepository.ObterInstituicoesVinculadas(idPratica);
            foreach (var vinculo in vinculosPratica)
            {
                await _praticaInstituicaoRepository.RemoverAsync([vinculo.idPratica, vinculo.idInstituicao]);
            }
            await _praticaRepository.RemoverAsync(idPratica);

            return Json(new { sucesso = true });
        }

        [HttpDelete]
        [Authorize(Policy = "ApenasInstituicao")]
        public async Task<JsonResult> DesvincularPratica(short idPratica, int idInstituicao)
        {
            var agendamentos = await _agendamentoRepository.ObterPorPraticaInstituicao(idPratica, idInstituicao);
            if (agendamentos.Any())
                return Json(new { sucesso = false, mensagem = "A prática não pode ser desvinculada, pois já existem agendamentos associados a ela." });

            await _praticaInstituicaoRepository.RemoverAsync([idPratica, idInstituicao]);

            return Json(new { sucesso = true });
        }
    }
}
