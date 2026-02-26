using AgendaPics.Application.Common.Interfaces;
using AgendaPics.Application.Common.Mediator;
using AgendaPics.Application.Features.Instituicoes.Queries;
using AgendaPics.Application.Features.Praticas.Commands;
using AgendaPics.Application.Features.Praticas.Queries;
using AgendaPics.Application.Features.PraticasInstituicoes.Commands;
using AgendaPics.Application.Features.PraticasInstituicoes.Queries;
using AgendaPics.Web.Areas.Admin.ViewModels.Pratica;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgendaPics.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class PraticaController : Controller
{
    private readonly IMediator _mediator;
    private readonly ITokenService _tokenService;

    public PraticaController(IMediator mediator, ITokenService tokenService)
    {
        _mediator = mediator;
        _tokenService = tokenService;
    }

    [HttpGet]
    public async Task<IActionResult> ObterPraticasPorInstituicao(int idInstituicao)
    {
        var query = new ObterPraticasQuery(idInstituicao);
        var result = await _mediator.Send(query);

        if (result.IsFailure || !result.Value.Any())
            return Json(new { sucesso = false, mensagem = "Nenhuma prática encontrada para a instituição informada." });

        var listaPraticas = result.Value.Select(p => new { idPratica = p.IdPratica, nome = p.Nome });
        return Json(new { sucesso = true, listaPraticas });
    }

    [HttpGet]
    [Authorize(Policy = "ApenasInstituicao")]
    public async Task<IActionResult> MinhasPraticas()
    {
        var usuarioInfo = _tokenService.ObterInformacoesToken();

        if (usuarioInfo?.IdInstituicao == null)
            return RedirectToAction("Login", "Login", new { area = "Admin" });

        ViewBag.idInstituicao = usuarioInfo.IdInstituicao.Value;

        var query = new ObterMinhasPraticasQuery(usuarioInfo.IdInstituicao.Value);
        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            TempData["Erro"] = result.Error;
            return View(new List<MinhasPraticasViewModel>());
        }

        var viewModel = result.Value.Select(p => new MinhasPraticasViewModel
        {
            idPratica = p.IdPratica,
            idInstituicao = usuarioInfo.IdInstituicao.Value,
            nome = p.Nome,
            periodicidade = p.Periodicidade,
            qtdSessoes = p.QtdSessoes,
            diaPermitidoParaAgendamento = p.DiaPermitidoParaAgendamento ?? 0,
            textoPeriodicidade = ObterTextoPeriodicidade(p.Periodicidade),
            textoQtdSessoes = $"{p.QtdSessoes} sessões",
            textoDiaPermitidoParaAgendamento = ObterTextoDiaSemana(p.DiaPermitidoParaAgendamento)
        }).ToList();

        return View(viewModel);
    }

    [HttpGet]
    [Authorize(Policy = "ApenasAdmin")]
    public async Task<IActionResult> MinhasPraticasAdmin()
    {
        var query = new ObterPraticasQuery();
        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            TempData["Erro"] = result.Error;
            return View(new List<PraticaViewModel>());
        }

        var viewModel = result.Value.Select(p => new PraticaViewModel
        {
            idPratica = p.IdPratica,
            nome = p.Nome,
            descricao = p.Descricao
        }).ToList();

        return View(viewModel);
    }

    [HttpGet]
    [Authorize(Policy = "ApenasAdmin")]
    public IActionResult NovaPratica()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Policy = "ApenasAdmin")]
    public async Task<IActionResult> CriarPratica(PraticaViewModel praticaViewModel)
    {
        var command = new CriarPraticaCommand(praticaViewModel.nome, praticaViewModel.descricao);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return Json(new { sucesso = false, mensagem = result.Error });

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
        var query = new ObterPraticaPorIdQuery(idPratica);
        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            TempData["Erro"] = result.Error;
            return RedirectToAction("MinhasPraticasAdmin");
        }

        var viewModel = new PraticaViewModel
        {
            idPratica = result.Value.IdPratica,
            nome = result.Value.Nome,
            descricao = result.Value.Descricao
        };

        return View(viewModel);
    }

    [HttpPut]
    [Authorize(Policy = "ApenasAdmin")]
    public async Task<IActionResult> EditarPratica(PraticaViewModel praticaViewModel)
    {
        var command = new EditarPraticaCommand(praticaViewModel.idPratica, praticaViewModel.nome, praticaViewModel.descricao);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return Json(new { sucesso = false, mensagem = result.Error });

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

        if (usuarioInfo?.IdInstituicao == null)
            return RedirectToAction("Login", "Login", new { area = "Admin" });

        ViewBag.idInstituicao = usuarioInfo.IdInstituicao.Value;

        var praticasQuery = new ObterPraticasQuery();
        var praticasResult = await _mediator.Send(praticasQuery);

        if (praticasResult.IsSuccess)
            ViewBag.Praticas = praticasResult.Value;

        return View();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> VincularPratica(VinculoPraticaViewModel vinculoPraticaViewModel)
    {
        var command = new VincularPraticaCommand(
            vinculoPraticaViewModel.idPratica,
            vinculoPraticaViewModel.idInstituicao,
            vinculoPraticaViewModel.periodicidade,
            vinculoPraticaViewModel.qtdSessoes,
            vinculoPraticaViewModel.diaPermitidoParaAgendamento);

        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return Json(new { sucesso = false, mensagem = result.Error });

        return Json(new { sucesso = true });
    }

    [HttpGet]
    [Authorize]
    public IActionResult VinculoPraticaSucesso()
    {
        var usuarioInfo = _tokenService.ObterInformacoesToken();
        ViewBag.idPerfil = usuarioInfo?.IdPerfil;

        return View();
    }

    [HttpGet]
    [Authorize(Policy = "ApenasInstituicao")]
    public async Task<IActionResult> EdicaoVinculoPratica(short idPratica)
    {
        var usuarioInfo = _tokenService.ObterInformacoesToken();

        if (usuarioInfo?.IdInstituicao == null)
            return RedirectToAction("Login", "Login", new { area = "Admin" });

        ViewBag.idInstituicao = usuarioInfo.IdInstituicao.Value;

        var praticasQuery = new ObterPraticasQuery();
        var praticasResult = await _mediator.Send(praticasQuery);

        if (praticasResult.IsSuccess)
            ViewBag.Praticas = praticasResult.Value;

        var praticaInstituicaoQuery = new ObterPraticaInstituicaoQuery(idPratica, usuarioInfo.IdInstituicao.Value);
        var praticaInstituicaoResult = await _mediator.Send(praticaInstituicaoQuery);

        if (praticaInstituicaoResult.IsFailure)
        {
            TempData["Erro"] = praticaInstituicaoResult.Error;
            return RedirectToAction("MinhasPraticas");
        }

        var viewModel = new VinculoPraticaViewModel
        {
            idPratica = praticaInstituicaoResult.Value.IdPratica,
            idInstituicao = praticaInstituicaoResult.Value.IdInstituicao,
            diaPermitidoParaAgendamento = praticaInstituicaoResult.Value.DiaPermitidoParaAgendamento,
            qtdSessoes = praticaInstituicaoResult.Value.QtdSessoes,
            periodicidade = praticaInstituicaoResult.Value.Periodicidade
        };

        return View(viewModel);
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> EditarVinculoPratica(VinculoPraticaViewModel vinculoPraticaViewModel)
    {
        var command = new EditarVinculoPraticaCommand(
            vinculoPraticaViewModel.idPratica,
            vinculoPraticaViewModel.idInstituicao,
            vinculoPraticaViewModel.periodicidade,
            vinculoPraticaViewModel.qtdSessoes,
            vinculoPraticaViewModel.diaPermitidoParaAgendamento);

        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return Json(new { sucesso = false, mensagem = result.Error });

        return Json(new { sucesso = true });
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
        var instituicoesQuery = new ObterInstituicoesQuery();
        var instituicoesResult = await _mediator.Send(instituicoesQuery);

        if (instituicoesResult.IsSuccess)
            ViewBag.Instituicoes = instituicoesResult.Value;

        var praticasQuery = new ObterPraticasQuery();
        var praticasResult = await _mediator.Send(praticasQuery);

        if (praticasResult.IsSuccess)
            ViewBag.Praticas = praticasResult.Value;

        return View();
    }

    [HttpDelete]
    [Authorize(Policy = "ApenasAdmin")]
    public async Task<IActionResult> ExcluirPratica(short idPratica)
    {
        var command = new ExcluirPraticaCommand(idPratica);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return Json(new { sucesso = false, mensagem = result.Error });

        return Json(new { sucesso = true });
    }

    [HttpDelete]
    [Authorize(Policy = "ApenasInstituicao")]
    public async Task<IActionResult> DesvincularPratica(short idPratica, int idInstituicao)
    {
        var command = new DesvincularPraticaCommand(idPratica, idInstituicao);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return Json(new { sucesso = false, mensagem = result.Error });

        return Json(new { sucesso = true });
    }

    private static string ObterTextoPeriodicidade(byte periodicidade)
    {
        return periodicidade switch
        {
            1 => "Semanal",
            2 => "Quinzenal",
            3 => "Mensal",
            _ => "Não definido"
        };
    }

    private static string ObterTextoDiaSemana(byte? diaSemana)
    {
        return diaSemana switch
        {
            0 => "Domingo",
            1 => "Segunda-feira",
            2 => "Terça-feira",
            3 => "Quarta-feira",
            4 => "Quinta-feira",
            5 => "Sexta-feira",
            6 => "Sábado",
            _ => "Não definido"
        };
    }
}
