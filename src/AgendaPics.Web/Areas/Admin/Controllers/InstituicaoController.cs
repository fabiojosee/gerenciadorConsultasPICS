using AgendaPics.Application.Common.Mediator;
using AgendaPics.Application.Features.Instituicoes.Commands;
using AgendaPics.Application.Features.Instituicoes.Queries;
using AgendaPics.Application.Features.Localidades.Queries;
using AgendaPics.Web.Areas.Admin.ViewModels.Instituicao;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgendaPics.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = "ApenasAdmin")]
public class InstituicaoController : Controller
{
    private readonly ILogger<InstituicaoController> _logger;
    private readonly IMediator _mediator;

    public InstituicaoController(
        ILogger<InstituicaoController> logger,
        IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> MinhasInstituicoes()
    {
        var query = new ObterInstituicoesQuery();
        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            TempData["Erro"] = result.Error;
            return View(Enumerable.Empty<MinhasInstituicoesViewModel>());
        }

        var viewModel = result.Value.Select(i => new MinhasInstituicoesViewModel
        {
            IdInstituicao = i.IdInstituicao,
            Nome = i.Nome,
            Descricao = i.Descricao
        });

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> NovaInstituicao()
    {
        var estadosQuery = new ObterEstadosQuery();
        var estadosResult = await _mediator.Send(estadosQuery);

        if (estadosResult.IsSuccess)
            ViewBag.Estados = estadosResult.Value;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CriarInstituicao(
        string nome,
        string? descricao,
        short idEstado,
        int idCidade,
        string cnpj,
        string cep,
        string email,
        TimeSpan horarioInicioAtendimento,
        TimeSpan horarioFimAtendimento)
    {
        var command = new CriarInstituicaoCommand(
            nome,
            descricao,
            idEstado,
            idCidade,
            cnpj,
            cep,
            email,
            horarioInicioAtendimento,
            horarioFimAtendimento);

        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return Json(new { sucesso = false, mensagem = result.Error });

        return Json(new { sucesso = true, idInstituicao = result.Value.IdInstituicao });
    }

    [HttpGet]
    public async Task<IActionResult> EdicaoInstituicao(int idInstituicao)
    {
        var query = new ObterInstituicoesQuery();
        var result = await _mediator.Send(query);

        var instituicao = result.Value?.FirstOrDefault(i => i.IdInstituicao == idInstituicao);
        if (instituicao == null)
        {
            TempData["Erro"] = "Instituição não encontrada.";
            return RedirectToAction("MinhasInstituicoes");
        }

        var estadosQuery = new ObterEstadosQuery();
        var estadosResult = await _mediator.Send(estadosQuery);

        if (estadosResult.IsSuccess)
            ViewBag.Estados = estadosResult.Value;

        var cidadeQuery = new ObterCidadeQuery(instituicao.IdCidade);
        var cidadeResult = await _mediator.Send(cidadeQuery);

        var cidadesQuery = new ObterCidadesQuery(cidadeResult.Value.IdEstado);
        var cidadesResult = await _mediator.Send(cidadesQuery);

        if (cidadesResult.IsSuccess)
            ViewBag.Cidades = cidadesResult.Value;

        var viewModel = new InstituicaoViewModel
        {
            idInstituicao = instituicao.IdInstituicao,
            nome = instituicao.Nome,
            descricao = instituicao.Descricao,
            idEstado = cidadeResult.Value.IdEstado,
            idCidade = instituicao.IdCidade,
            cnpj = instituicao.Cnpj,
            cep = instituicao.Cep,
            email = instituicao.Email,
            horarioInicioAtendimento = instituicao.HorarioInicioAtendimento,
            horarioFimAtendimento = instituicao.HorarioFimAtendimento
        };

        return View(viewModel);
    }

    [HttpPut]
    public async Task<IActionResult> EditarInstituicao(
        int idInstituicao,
        string nome,
        string? descricao,
        short idEstado,
        int idCidade,
        string cnpj,
        string cep,
        string email,
        TimeSpan horarioInicioAtendimento,
        TimeSpan horarioFimAtendimento)
    {
        var command = new EditarInstituicaoCommand(
            idInstituicao,
            nome,
            descricao,
            idEstado,
            idCidade,
            cnpj,
            cep,
            email,
            horarioInicioAtendimento,
            horarioFimAtendimento);

        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return Json(new { sucesso = false, mensagem = result.Error });

        return Json(new { sucesso = true });
    }

    [HttpDelete]
    public async Task<IActionResult> ExcluirInstituicao(int id)
    {
        var command = new ExcluirInstituicaoCommand(id);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return Json(new { sucesso = false, mensagem = result.Error });

        return Json(new { sucesso = true });
    }

    [HttpGet]
    public IActionResult NovaInstituicaoSucesso()
    {
        return View();
    }

    [HttpGet]
    public IActionResult EdicaoInstituicaoSucesso()
    {
        return View();
    }
}
