using gerenciadorConsultasPICS.Areas.Admin.Enums;
using gerenciadorConsultasPICS.Areas.Admin.Models;
using gerenciadorConsultasPICS.Areas.Admin.ViewModels.Instituicao;
using gerenciadorConsultasPICS.Helpers;
using gerenciadorConsultasPICS.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace gerenciadorConsultasPICS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class InstituicaoController : Controller
    {
        private readonly ILogger<InstituicaoController> _logger;
        private readonly IPraticaInstituicaoRepository _praticaInstituicaoRepository;
        private readonly IInstituicaoRepository _instituicaoRepository;
        private readonly IAgendamentoRepository _agendamentoRepository;
        private readonly IEstadoRepository _estadoRepository;
        private readonly ICidadeRepository _cidadeRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public InstituicaoController(
            ILogger<InstituicaoController> logger,
            IPraticaInstituicaoRepository praticaInstituicaoRepository,
            IInstituicaoRepository instituicaoRepository,
            IAgendamentoRepository agendamentoRepository,
            IEstadoRepository estadoRepository,
            ICidadeRepository cidadeRepository,
            IUsuarioRepository usuarioRepository)
        {
            _logger = logger;
            _praticaInstituicaoRepository = praticaInstituicaoRepository;
            _instituicaoRepository = instituicaoRepository;
            _agendamentoRepository = agendamentoRepository;
            _estadoRepository = estadoRepository;
            _cidadeRepository = cidadeRepository;
            _usuarioRepository = usuarioRepository;
        }

        [HttpGet]
        [Authorize(Policy = "ApenasAdmin")]
        public async Task<IActionResult> MinhasInstituicoes()
        {
            var instituicoes = await _instituicaoRepository.ObterTodosAsync();

            return View(instituicoes);
        }

        [HttpGet]
        [Authorize(Policy = "ApenasAdmin")]
        public async Task<IActionResult> NovaInstituicao()
        {
            ViewBag.Estados = await _estadoRepository.ObterTodosAsync();

            return View();
        }

        [HttpPost]
        [Authorize(Policy = "ApenasAdmin")]
        public async Task<JsonResult> CriarInstituicao(InstituicaoViewModel instituicaoViewModel)
        {
            if (instituicaoViewModel is null)
                return Json(new { sucesso = false, mensagem = "Formato inválido." });

            if (instituicaoViewModel.nome is null)
                return Json(new { sucesso = false, mensagem = "O campo nome é obrigatório." });

            if (instituicaoViewModel.idEstado is null)
                return Json(new { sucesso = false, mensagem = "A seleção de estado é obrigatória." });

            if (instituicaoViewModel.idCidade is null)
                return Json(new { sucesso = false, mensagem = "A seleção de cidade é obrigatória." });

            if (instituicaoViewModel.cnpj is null)
                return Json(new { sucesso = false, mensagem = "O campo CNPJ é obrigatório." });

            instituicaoViewModel.cnpj = Regex.Replace(instituicaoViewModel.cnpj, @"\D", "");
            if (instituicaoViewModel.cnpj.Length > 14)
                return Json(new { sucesso = false, mensagem = "O CNPJ deve possuir no máximo 14 caracteres." });

            instituicaoViewModel.cep = Regex.Replace(instituicaoViewModel.cep, @"\D", "");
            if (instituicaoViewModel.cep is null)
                return Json(new { sucesso = false, mensagem = "O campo CEP é obrigatório." });

            if (instituicaoViewModel.email is null)
                return Json(new { sucesso = false, mensagem = "O campo e-mail é obrigatório." });

            if (instituicaoViewModel.horarioInicioAtendimento is null || instituicaoViewModel.horarioFimAtendimento is null)
                return Json(new { sucesso = false, mensagem = "Preencha os horários de início e fim dos atendimentos." });

            Instituicao instituicao = Instituicao.InstituicaoFactory.CriarInstituicao(
                instituicaoViewModel.nome,
                instituicaoViewModel.descricao,
                instituicaoViewModel.idEstado.Value,
                instituicaoViewModel.idCidade.Value,
                instituicaoViewModel.cnpj,
                instituicaoViewModel.cep,
                instituicaoViewModel.email,
                instituicaoViewModel.horarioInicioAtendimento.Value,
                instituicaoViewModel.horarioFimAtendimento.Value);

            await _instituicaoRepository.AdicionarAsync(instituicao);

            var novoUsuario = Models.Usuario.UsuarioFactory.CriarUsuario(
                (byte)Perfil.Instituicao,
                instituicao.idInstituicao,
                instituicao.cnpj,
                HashHelper.Criptografar(instituicao.cnpj)
            );

            await _usuarioRepository.AdicionarAsync(novoUsuario);

            return Json(new { sucesso = true });
        }

        [HttpGet]
        [Authorize(Policy = "ApenasAdmin")]
        public IActionResult NovaInstituicaoSucesso()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Policy = "ApenasAdmin")]
        public IActionResult AvisoLogin()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Policy = "ApenasAdmin")]
        public async Task<IActionResult> EdicaoInstituicao(int idInstituicao)
        {
            var instituicao = await _instituicaoRepository.ObterPorIdAsync(idInstituicao);

            ViewBag.Estados = await _estadoRepository.ObterTodosAsync();
            ViewBag.Cidades = await _cidadeRepository.ObterPorEstado(instituicao.idEstado);

            return View(new InstituicaoViewModel()
            {
                idInstituicao = instituicao.idInstituicao,
                nome = instituicao.nome,
                descricao = instituicao.descricao,
                idEstado = instituicao.idEstado,
                idCidade = instituicao.idCidade,
                cnpj = instituicao.cnpj,
                cep = instituicao.cep,
                email = instituicao.email,
                horarioInicioAtendimento = instituicao.horarioInicioAtendimento,
                horarioFimAtendimento = instituicao.horarioFimAtendimento
            });
        }

        [HttpPut]
        [Authorize(Policy = "ApenasAdmin")]
        public async Task<JsonResult> EditarInstituicao(InstituicaoViewModel instituicaoViewModel)
        {
            if (instituicaoViewModel is null)
                return Json(new { sucesso = false, mensagem = "Formato inválido." });

            if (instituicaoViewModel.nome is null)
                return Json(new { sucesso = false, mensagem = "O campo nome é obrigatório." });

            if (instituicaoViewModel.idEstado is null)
                return Json(new { sucesso = false, mensagem = "A seleção de estado é obrigatória." });

            if (instituicaoViewModel.idCidade is null)
                return Json(new { sucesso = false, mensagem = "A seleção de cidade é obrigatória." });

            if (instituicaoViewModel.cnpj is null)
                return Json(new { sucesso = false, mensagem = "O campo CNPJ é obrigatório." });

            instituicaoViewModel.cnpj = Regex.Replace(instituicaoViewModel.cnpj, @"\D", "");
            if (instituicaoViewModel.cnpj.Length > 14)
                return Json(new { sucesso = false, mensagem = "O CNPJ deve possuir no máximo 14 caracteres." });

            instituicaoViewModel.cep = Regex.Replace(instituicaoViewModel.cep, @"\D", "");
            if (instituicaoViewModel.cep is null)
                return Json(new { sucesso = false, mensagem = "O campo CEP é obrigatório." });

            if (instituicaoViewModel.email is null)
                return Json(new { sucesso = false, mensagem = "O campo e-mail é obrigatório." });

            if (instituicaoViewModel.horarioInicioAtendimento is null || instituicaoViewModel.horarioFimAtendimento is null)
                return Json(new { sucesso = false, mensagem = "Preencha os horários de início e fim dos atendimentos." });

            var instituicao = await _instituicaoRepository.ObterPorIdAsync(instituicaoViewModel.idInstituicao.Value);
            if (instituicao is null)
                return Json(new { sucesso = false, mensagem = "A instituição não foi encontrada." });

            instituicao.Atualizar(
                instituicaoViewModel.nome,
                instituicaoViewModel.descricao,
                instituicaoViewModel.idEstado.Value,
                instituicaoViewModel.idCidade.Value,
                instituicaoViewModel.cnpj,
                instituicaoViewModel.cep,
                instituicaoViewModel.email,
                instituicaoViewModel.horarioInicioAtendimento.Value,
                instituicaoViewModel.horarioFimAtendimento.Value);

            await _instituicaoRepository.AtualizarAsync(instituicao);

            return Json(new { sucesso = true });
        }

        [HttpGet]
        [Authorize(Policy = "ApenasAdmin")]
        public IActionResult EdicaoInstituicaoSucesso()
        {
            return View();
        }

        [HttpDelete]
        [Authorize(Policy = "ApenasAdmin")]
        public async Task<JsonResult> ExcluirInstituicao(int idInstituicao)
        {
            var agendamentos = await _agendamentoRepository.ObterPorInstituicao(idInstituicao);
            if (agendamentos.Any())
                return Json(new { sucesso = false, mensagem = "A instituição não pode ser excluída, pois já existem agendamentos associados a ela." });

            var usuario = await _usuarioRepository.ObterPorInstituicao(idInstituicao);
            if (usuario is not null)
                await _usuarioRepository.RemoverAsync(usuario.idUsuario);

            var vinculosPratica = await _praticaInstituicaoRepository.ObterPraticasVinculadas(idInstituicao);
            foreach (var vinculo in vinculosPratica)
            {
                await _praticaInstituicaoRepository.RemoverAsync([vinculo.idPratica, vinculo.idInstituicao]);
            }
            await _instituicaoRepository.RemoverAsync(idInstituicao);

            return Json(new { sucesso = true });
        }
    }
}
