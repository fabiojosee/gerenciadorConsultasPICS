using gerenciadorConsultasPICS.Areas.Admin.Enums;
using gerenciadorConsultasPICS.Areas.Usuario.Models;
using gerenciadorConsultasPICS.Repositories.Interfaces;
using gerenciadorConsultasPICS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace gerenciadorConsultasPICS.Areas.Usuario.Controllers
{
    [Area("Usuario")]
    public class AtendimentoController : Controller
    {
        private readonly ILogger<AgendamentoController> _logger;
        private readonly IAtendimentoRepository _atendimentoRepository;
        private readonly IAgendamentoRepository _agendamentoRepository;
        private readonly IAvaliacaoRepository _avaliacaoRepository;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;

        public AtendimentoController(
            ILogger<AgendamentoController> logger,
            IAtendimentoRepository atendimentoRepository,
            IAgendamentoRepository agendamentoRepository,
            IAvaliacaoRepository avaliacaoRepository,
            ITokenService tokenService,
            IConfiguration configuration)
        {
            _logger = logger;
            _atendimentoRepository = atendimentoRepository;
            _agendamentoRepository = agendamentoRepository;
            _avaliacaoRepository = avaliacaoRepository;
            _tokenService = tokenService;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> MeusAtendimentos(string cpfPaciente)
        {
            cpfPaciente = Regex.Replace(cpfPaciente, @"\D", "");
            var atendimentos = await _atendimentoRepository.ObterPorCpfPaciente(cpfPaciente);

            ViewBag.FormularioAvaliacao = _configuration["Formularios:Avaliacao"];

            return View(atendimentos);
        }

        [HttpPut]
        public async Task<IActionResult> CancelarAtendimento(int idAtendimento)
        {
            var atendimento = await _atendimentoRepository.ObterPorIdAsync(idAtendimento);
            if (atendimento is null)
                return Json(new { sucesso = false, mensagem = "Atendimento não encontrado." });

            var agendamento = await _agendamentoRepository.ObterPorIdAsync(atendimento.idAgendamento);
            if (agendamento is null)
                return Json(new { sucesso = false, mensagem = "Agendamento não encontrado." });

            var atendimentosAgendados = await _atendimentoRepository.ObterPorAgendamento(atendimento.idAgendamento);
            foreach (var atendimentoAgendado in atendimentosAgendados)
            {
                atendimentoAgendado.AlterarStatus((byte)StatusAtendimento.Cancelado);
                await _atendimentoRepository.AtualizarAsync(atendimentoAgendado);
            }

            agendamento.AlterarStatus((byte)StatusAgendamento.Cancelado);
            await _agendamentoRepository.AtualizarAsync(agendamento);

            return Json(new { sucesso = true });
        }

        [HttpGet]
        [Authorize(Policy = "ApenasInstituicao")]
        public async Task<IActionResult> MeusAtendimentosInstituicao()
        {
            var usuarioInfo = _tokenService.ObterInformacoesToken();

            var atendimentos = await _atendimentoRepository.ObterPorInstituicao(usuarioInfo.idInstituicao.Value);

            return View(atendimentos);
        }

        [HttpPut]
        [Authorize(Policy = "ApenasInstituicao")]
        public async Task<IActionResult> FinalizarAtendimento(int idAtendimento)
        {
            var atendimento = await _atendimentoRepository.ObterPorIdAsync(idAtendimento);
            if (atendimento is null)
                return Json(new { sucesso = false, mensagem = "Atendimento não encontrado." });

            var agendamento = await _agendamentoRepository.ObterPorIdAsync(atendimento.idAgendamento);
            if (agendamento is null)
                return Json(new { sucesso = false, mensagem = "Agendamento não encontrado." });

            atendimento.AlterarStatus((byte)StatusAtendimento.Finalizado);
            await _atendimentoRepository.AtualizarAsync(atendimento);

            var atendimentosAgendados = await _atendimentoRepository.ObterPorAgendamento(atendimento.idAgendamento);
            if (atendimentosAgendados.All(x => x.status == (byte)StatusAtendimento.Finalizado))
            {
                agendamento.AlterarStatus((byte)StatusAgendamento.Concluido);
                await _agendamentoRepository.AtualizarAsync(agendamento);
            }

            return Json(new { sucesso = true });
        }

        [HttpPost]
        public async Task<IActionResult> AvaliarAtendimento(int idAtendimento, string link)
        {
            var atendimento = await _atendimentoRepository.ObterPorIdAsync(idAtendimento);
            if (atendimento is null)
                return Json(new { sucesso = false, mensagem = "Atendimento não encontrado." });

            var avaliacao = Avaliacao.AvaliacaoFactory.CriarAvaliacao(
                    idAtendimento,
                    DateTime.Now,
                    link,
                    "Usuário acessou avaliação."
                );

            await _avaliacaoRepository.AdicionarAsync(avaliacao);

            return Json(new { sucesso = true });
        }
    }
}
