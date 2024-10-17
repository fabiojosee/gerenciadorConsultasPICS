using gerenciadorConsultasPICS.Areas.Admin.Enums;
using gerenciadorConsultasPICS.Repositories.Interfaces;
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

        public AtendimentoController(
            ILogger<AgendamentoController> logger,
            IAtendimentoRepository atendimentoRepository,
            IAgendamentoRepository agendamentoRepository)
        {
            _logger = logger;
            _atendimentoRepository = atendimentoRepository;
            _agendamentoRepository = agendamentoRepository;
        }

        [HttpGet]
        public async Task<IActionResult> MeusAtendimentos(string cpfPaciente)
        {
            cpfPaciente = Regex.Replace(cpfPaciente, @"\D", "");

            var atendimentos = await _atendimentoRepository.ObterPorCpfPaciente(cpfPaciente);

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
    }
}
