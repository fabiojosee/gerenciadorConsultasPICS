using AgendaPics.Application.Common.Interfaces;
using AgendaPics.Application.Common.Mediator;
using AgendaPics.Application.Features.Atendimentos.Commands;
using AgendaPics.Application.Features.Atendimentos.Queries;
using AgendaPics.Application.Features.Autenticacao.Commands;
using AgendaPics.Web.Areas.Admin.ViewModels.Pratica;
using AgendaPics.Web.Areas.Usuario.ViewModels.Atendimento;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgendaPics.Web.Areas.Usuario.Controllers;

[Area("Usuario")]
public class AtendimentoController : Controller
{
    private readonly ILogger<AtendimentoController> _logger;
    private readonly IMediator _mediator;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    public AtendimentoController(
        ILogger<AtendimentoController> logger,
        IMediator mediator,
        ITokenService tokenService,
        IConfiguration configuration)
    {
        _logger = logger;
        _mediator = mediator;
        _tokenService = tokenService;
        _configuration = configuration;
    }

    #region Ações do Paciente (Acesso via Token)

    [HttpGet]
    public IActionResult SolicitarAcesso()
    {
        var cpf = TempData["CpfAcesso"]?.ToString();
        TempData.Keep("CpfAcesso");
        ViewBag.Cpf = cpf;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SolicitarAcesso(string cpf, string email)
    {
        var command = new SolicitarAcessoCommand(cpf, email);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return Json(new { sucesso = false, mensagem = result.Error });

        TempData["CpfAcesso"] = cpf;
        return Json(new { sucesso = true });
    }

    [HttpGet]
    public IActionResult ValidarToken()
    {
        var cpf = TempData["CpfAcesso"]?.ToString();
        TempData.Keep("CpfAcesso");

        if (string.IsNullOrEmpty(cpf))
            return RedirectToAction("SolicitarAcesso");

        ViewBag.Cpf = cpf;
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> MeusAtendimentos(string cpf, string token)
    {
        TempData["CpfAcesso"] = cpf;
        if (string.IsNullOrWhiteSpace(cpf) || string.IsNullOrWhiteSpace(token))
            return RedirectToAction("SolicitarAcesso");

        var reutilizarToken = Convert.ToBoolean(TempData["ReutilizarToken"]);

        var query = new ObterAtendimentosPorCpfQuery(cpf, token, reutilizarToken);
        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            TempData["Erro"] = result.Error;
            return RedirectToAction("SolicitarAcesso");
        }

        ViewBag.FormularioAvaliacao = _configuration["Formularios:Avaliacao"];
        ViewBag.Cpf = cpf;

        var viewModel = result.Value.Select(i => new MeusAtendimentosViewModel(
            i.IdAtendimento,
            i.NomePratica,
            i.CidadePaciente,
            i.EstadoPaciente,
            i.DataAtendimento,
            i.StatusDescricao,
            i.Status,
            i.HorarioInicioAtendimento,
            i.HorarioFimAtendimento,
            i.NomeInstituicao,
            i.CepInstituicao
        ));

        return View(viewModel);
    }

    #endregion

    #region Ações do Paciente (Authorized)

    [HttpPut]
    public async Task<IActionResult> CancelarAtendimento(int idAtendimento)
    {
        var command = new CancelarAtendimentoCommand(idAtendimento, "Cancelado pelo paciente.");
        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return Json(new { sucesso = false, mensagem = result.Error });

        TempData["ReutilizarToken"] = true;
        return Json(new { sucesso = true });
    }

    [HttpPost]
    public async Task<IActionResult> AvaliarAtendimento(int idAtendimento, string link)
    {
        var command = new AvaliarAtendimentoCommand(idAtendimento, link, "Usuário acessou avaliação.");
        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return Json(new { sucesso = false, mensagem = result.Error });

        return Json(new { sucesso = true });
    }

    #endregion

    #region Ações da Instituição (Authorized)

    [HttpGet]
    [Authorize(Policy = "ApenasInstituicao")]
    public async Task<IActionResult> MeusAtendimentosInstituicao()
    {
        var usuarioInfo = _tokenService.ObterInformacoesToken();

        if (usuarioInfo?.IdInstituicao == null)
            return RedirectToAction("Login", "Login", new { area = "Admin" });

        var query = new ObterAtendimentosPorInstituicaoQuery(usuarioInfo.IdInstituicao.Value);
        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            TempData["Erro"] = result.Error;
            return View(new List<MeusAtendimentosInstituicaoViewModel>());
        }
        var viewModel = result.Value.Select(i => new MeusAtendimentosInstituicaoViewModel(
            i.IdAtendimento,
            i.NomePratica,
            i.CidadePaciente,
            i.EstadoPaciente,
            i.DataAtendimento,
            i.StatusDescricao,
            i.Status,
            i.NomePaciente,
            i.TelefonePaciente,
            i.DataNascimentoPaciente
        ));

        return View(viewModel);
    }

    [HttpPut]
    [Authorize(Policy = "ApenasInstituicao")]
    public async Task<IActionResult> FinalizarAtendimento(int idAtendimento, string? observacao)
    {
        var command = new FinalizarAtendimentoCommand(idAtendimento, observacao);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return Json(new { sucesso = false, mensagem = result.Error });

        return Json(new { sucesso = true });
    }

    [HttpPut]
    [Authorize(Policy = "ApenasInstituicao")]
    public async Task<IActionResult> CancelarAtendimentoInstituicao(int idAtendimento, string? observacao)
    {
        var command = new CancelarAtendimentoCommand(idAtendimento, observacao ?? "Cancelado pela instituição.");
        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return Json(new { sucesso = false, mensagem = result.Error });

        return Json(new { sucesso = true });
    }

    #endregion
}
