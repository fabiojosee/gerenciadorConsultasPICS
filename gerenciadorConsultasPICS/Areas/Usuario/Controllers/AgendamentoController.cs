using gerenciadorConsultasPICS.Areas.Admin.Enums;
using gerenciadorConsultasPICS.Areas.Usuario.Models;
using gerenciadorConsultasPICS.Areas.Usuario.ViewModels.Agendamento;
using gerenciadorConsultasPICS.Helpers;
using gerenciadorConsultasPICS.Repositories.Interfaces;
using gerenciadorConsultasPICS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace gerenciadorConsultasPICS.Areas.Usuario.Controllers
{
    [Area("Usuario")]
    public class AgendamentoController : Controller
    {
        private readonly ILogger<AgendamentoController> _logger;
        private readonly IAgendamentoRepository _agendamentoRepository;
        private readonly IAtendimentoRepository _atendimentoRepository;
        private readonly IInstituicaoRepository _instituicaoRepository;
        private readonly IEstadoRepository _estadoRepository;
        private readonly IPraticaInstituicaoRepository _praticaInstituicaoRepository;
        private readonly IEmailService _emailService;

        public AgendamentoController(
            ILogger<AgendamentoController> logger,
            IAgendamentoRepository agendamentoRepository,
            IAtendimentoRepository atendimentoRepository,
            IInstituicaoRepository instituicaoRepository,
            IEstadoRepository estadoRepository,
            IPraticaInstituicaoRepository praticaInstituicaoRepository,
            IEmailService emailService)
        {
            _logger = logger;
            _agendamentoRepository = agendamentoRepository;
            _atendimentoRepository = atendimentoRepository;
            _instituicaoRepository = instituicaoRepository;
            _estadoRepository = estadoRepository;
            _praticaInstituicaoRepository = praticaInstituicaoRepository;
            _emailService = emailService;
        }

        [HttpGet]
        public async Task<IActionResult> NovoAgendamentoEtapa1()
        {
            ViewBag.Estados = await _estadoRepository.ObterTodosAsync();

            Etapa1ViewModel etapaAtual = new Etapa1ViewModel()
            {
                idCidadePaciente = TempData["NovoAgendamento_idCidadePaciente"] is null ? null : Convert.ToInt32(TempData["NovoAgendamento_idCidadePaciente"]),
                idEstadoPaciente = TempData["NovoAgendamento_idEstadoPaciente"] is null ? null : Convert.ToInt16(TempData["NovoAgendamento_idEstadoPaciente"])
            };
            TempData.Keep();

            return View(etapaAtual);
        }

        [HttpPost]
        public JsonResult NovoAgendamentoEtapa1(Etapa1ViewModel novoAgendamento)
        {
            TempData.Keep();

            if (novoAgendamento is null)
                return Json(new { sucesso = false, mensagem = "Formato inválido." });

            if (novoAgendamento.idEstadoPaciente is null)
                return Json(new { sucesso = false, mensagem = "A seleção de estado é obrigatória." });

            if (novoAgendamento.idCidadePaciente is null)
                return Json(new { sucesso = false, mensagem = "A seleção de cidade é obrigatória." });

            TempData["NovoAgendamento_idEstadoPaciente"] = novoAgendamento.idEstadoPaciente;
            TempData["NovoAgendamento_idCidadePaciente"] = novoAgendamento.idCidadePaciente;

            return Json(new { sucesso = true });
        }

        [HttpGet]
        public async Task<IActionResult> NovoAgendamentoEtapa2()
        {
            short idEstadoPaciente = Convert.ToInt16(TempData["NovoAgendamento_idEstadoPaciente"]);
            ViewBag.Instituicoes = await _instituicaoRepository.ObterPorEstado(idEstadoPaciente);

            Etapa2ViewModel etapaAtual = new Etapa2ViewModel()
            {
                idInstituicao = TempData["NovoAgendamento_idInstituicao"] is null ? null : Convert.ToInt32(TempData["NovoAgendamento_idInstituicao"]),
                idPratica = TempData["NovoAgendamento_idPratica"] is null ? null : Convert.ToInt16(TempData["NovoAgendamento_idPratica"])
            };
            TempData.Keep();

            return View(etapaAtual);
        }

        [HttpPost]
        public IActionResult NovoAgendamentoEtapa2(Etapa2ViewModel novoAgendamento)
        {
            TempData.Keep();
            if (novoAgendamento is null)
                return Json(new { sucesso = false, mensagem = "Formato inválido." });

            if (novoAgendamento.idInstituicao is null)
                return Json(new { sucesso = false, mensagem = "A seleção de instituição é obrigatória." });

            if (novoAgendamento.idPratica is null)
                return Json(new { sucesso = false, mensagem = "A seleção de prática é obrigatória." });

            TempData["NovoAgendamento_idInstituicao"] = novoAgendamento.idInstituicao;
            TempData["NovoAgendamento_idPratica"] = novoAgendamento.idPratica.ToString();

            return Json(new { sucesso = true });
        }

        [HttpGet]
        public async Task<IActionResult> NovoAgendamentoEtapa3()
        {
            Etapa3ViewModel etapaAtual = new Etapa3ViewModel()
            {
                dataInicio = TempData["NovoAgendamento_dataInicio"] is null ? null : Convert.ToDateTime(TempData["NovoAgendamento_dataInicio"])
            };

            int idInstituicao = Convert.ToInt32(TempData["NovoAgendamento_idInstituicao"]);
            short idPratica = Convert.ToInt16(TempData["NovoAgendamento_idPratica"]);
            TempData.Keep();

            var praticaInstituicao = await _praticaInstituicaoRepository.ObterPorPraticaInstituicao(idInstituicao, idPratica);
            ViewBag.DiaPermitidoParaAgendamento = praticaInstituicao.diaPermitidoParaAgendamento;

            return View(etapaAtual);
        }

        [HttpPost]
        public IActionResult NovoAgendamentoEtapa3(Etapa3ViewModel novoAgendamento)
        {
            TempData.Keep();
            if (novoAgendamento is null)
                return Json(new { sucesso = false, mensagem = "Formato inválido." });

            if (novoAgendamento.dataInicio is null)
                return Json(new { sucesso = false, mensagem = "Selecione uma data para prosseguir." });

            TempData["NovoAgendamento_dataInicio"] = novoAgendamento.dataInicio;

            return Json(new { sucesso = true });
        }

        [HttpGet]
        public IActionResult NovoAgendamentoEtapa4()
        {
            TempData.Keep();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NovoAgendamentoEtapa4(Etapa4ViewModel novoAgendamentoViewModel)
        {
            TempData.Keep();
            if (novoAgendamentoViewModel is null)
                return Json(new { sucesso = false, mensagem = "Formato inválido." });

            if (string.IsNullOrEmpty(novoAgendamentoViewModel.nomePaciente))
                return Json(new { sucesso = false, mensagem = "Preencha o campo Nome." });

            if (string.IsNullOrEmpty(novoAgendamentoViewModel.cpfPaciente))
                return Json(new { sucesso = false, mensagem = "Preencha o campo CPF." });

            novoAgendamentoViewModel.cpfPaciente = Regex.Replace(novoAgendamentoViewModel.cpfPaciente, @"\D", "");
            if (novoAgendamentoViewModel.cpfPaciente.Length > 11)
                return Json(new { sucesso = false, mensagem = "O CPF deve possuir no máximo 11 caracteres." });

            if (string.IsNullOrEmpty(novoAgendamentoViewModel.telefonePaciente))
                return Json(new { sucesso = false, mensagem = "Preencha o campo Telefone." });

            novoAgendamentoViewModel.telefonePaciente = Regex.Replace(novoAgendamentoViewModel.telefonePaciente, @"\D", "");
            if (novoAgendamentoViewModel.telefonePaciente.Length > 11)
                return Json(new { sucesso = false, mensagem = "O telefone deve possuir no máximo 11 caracteres." });

            if (string.IsNullOrEmpty(novoAgendamentoViewModel.emailPaciente))
                return Json(new { sucesso = false, mensagem = "Preencha o campo E-mail." });

            if (novoAgendamentoViewModel.dataNascimentoPaciente is null)
                return Json(new { sucesso = false, mensagem = "Preencha o campo Data de nascimento." });

            if (novoAgendamentoViewModel.generoPaciente is null)
                return Json(new { sucesso = false, mensagem = "Preencha o campo Gênero." });

            if (novoAgendamentoViewModel.grauAnsiedadePaciente is null)
                return Json(new { sucesso = false, mensagem = "Preencha o campo Grau de ansiedade." });

            #region Criação agendamento

            short idEstadoPaciente = Convert.ToInt16(TempData["NovoAgendamento_idEstadoPaciente"]);
            int idCidadePaciente = Convert.ToInt32(TempData["NovoAgendamento_idCidadePaciente"]);
            int idInstituicao = Convert.ToInt32(TempData["NovoAgendamento_idInstituicao"]);
            short idPratica = Convert.ToInt16(TempData["NovoAgendamento_idPratica"]);
            DateTime dataInicio = Convert.ToDateTime(TempData["NovoAgendamento_dataInicio"]);
            TempData.Keep();

            var agendamentos = await _agendamentoRepository.ObterPorPaciente(idPratica, novoAgendamentoViewModel.cpfPaciente, (byte)StatusAgendamento.EmAndamento);
            if (agendamentos.Any())
                return Json(new { sucesso = false, mensagem = "Você já possui um agendamento em andamento para a prática selecionada." });

            Agendamento novoAgendamento = Agendamento.AgendamentoFactory.CriarAgendamento(
                idInstituicao,
                idPratica,
                novoAgendamentoViewModel.nomePaciente,
                novoAgendamentoViewModel.cpfPaciente,
                novoAgendamentoViewModel.telefonePaciente,
                novoAgendamentoViewModel.dataNascimentoPaciente.Value,
                novoAgendamentoViewModel.generoPaciente.Value,
                novoAgendamentoViewModel.emailPaciente,
                idEstadoPaciente,
                idCidadePaciente,
                novoAgendamentoViewModel.grauAnsiedadePaciente.Value
            );

            await _agendamentoRepository.AdicionarAsync(novoAgendamento);

            #endregion

            #region Criação atendimentos

            var praticaInstituicao = await _praticaInstituicaoRepository.ObterPorPraticaInstituicao(idInstituicao, idPratica);

            List<DateTime> datasAgendadas = [];

            for (int i = 0; i < praticaInstituicao.qtdSessoes; i++)
            {
                datasAgendadas.Add(dataInicio);

                Atendimento novoAtendimento = Atendimento.AtendimentoFactory.CriarAtendimento(novoAgendamento.idAgendamento, dataInicio, $"Grau de ansiedade: {novoAgendamentoViewModel.grauAnsiedadePaciente.Value}");

                await _atendimentoRepository.AdicionarAsync(novoAtendimento);

                switch (praticaInstituicao.periodicidade)
                {
                    case 1:
                        dataInicio = dataInicio.AddDays(1);
                        break;
                    case 2:
                        dataInicio = dataInicio.AddDays(7);
                        break;
                    case 3:
                        dataInicio = dataInicio.AddMonths(1);
                        break;
                    default:
                        break;
                }
            }

            #endregion

            #region Enviar notificação

            List<string> linhasMensagem = new List<string>();

            foreach (var dataAgendada in datasAgendadas)
            {
                linhasMensagem.Add(dataAgendada.ToShortDateString());
            }
            var instituicao = await _instituicaoRepository.ObterPorIdAsync(idInstituicao);
            linhasMensagem.Add($"<br>Nome da instituição: {instituicao.nome} - CEP: {FormatarCep(instituicao.cep)}.");
            linhasMensagem.Add($"O atendimento é realizado por ordem de chegada, das {instituicao.horarioInicioAtendimento.ToString(@"hh\:mm")} às {instituicao.horarioFimAtendimento.ToString(@"hh\:mm")}.");

            var mensagem = EmailHelper.GerarTemplateEmail("Você possui novos atendimentos agendados para as seguintes datas:", linhasMensagem);

            await _emailService.EnviarEmailAsync(novoAgendamentoViewModel.emailPaciente, "Agendamento realizado com sucesso!", mensagem);

            #endregion

            return Json(new { sucesso = true });
        }

        public IActionResult Sucesso()
        {
            return View();
        }

        #region Métodos auxiliares

        private string FormatarCep(string cep)
        {
            cep = new string(cep.Where(char.IsDigit).ToArray());
            if (cep.Length == 8)
                return $"{cep.Substring(0, 5)}-{cep.Substring(5, 3)}";

            return cep;
        }

        #endregion
    }
}
