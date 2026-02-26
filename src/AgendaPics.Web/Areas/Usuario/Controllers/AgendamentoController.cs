using AgendaPics.Application.Common.Mediator;
using AgendaPics.Application.Features.Agendamentos.Commands;
using AgendaPics.Application.Features.Instituicoes.Queries;
using AgendaPics.Application.Features.Localidades.Queries;
using AgendaPics.Application.Features.Praticas.Queries;
using AgendaPics.Application.Features.PraticasInstituicoes.Queries;
using AgendaPics.Web.Areas.Usuario.ViewModels.Agendamento;
using Microsoft.AspNetCore.Mvc;

namespace AgendaPics.Web.Areas.Usuario.Controllers;

[Area("Usuario")]
public class AgendamentoController : Controller
{
    private readonly ILogger<AgendamentoController> _logger;
    private readonly IMediator _mediator;

    public AgendamentoController(
        ILogger<AgendamentoController> logger,
        IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> NovoAgendamentoEtapa1()
    {
        var estadosQuery = new ObterEstadosQuery();
        var estadosResult = await _mediator.Send(estadosQuery);

        if (estadosResult.IsSuccess)
            ViewBag.Estados = estadosResult.Value;

        Etapa1ViewModel etapaAtual = new Etapa1ViewModel()
        {
            idCidadePaciente = TempData["idCidadePaciente"] is null ? null : Convert.ToInt32(TempData["idCidadePaciente"]),
            idEstadoPaciente = TempData["idEstadoPaciente"] is null ? null : Convert.ToInt16(TempData["idEstadoPaciente"])
        };
        TempData.Keep();

        return View(etapaAtual);
    }

    [HttpPost]
    public async Task<IActionResult> NovoAgendamentoEtapa1(short idEstadoPaciente, int idCidadePaciente)
    {
        TempData["idEstadoPaciente"] = idEstadoPaciente.ToString();
        TempData["idCidadePaciente"] = idCidadePaciente.ToString();

        return Json(new { sucesso = true });
    }

    [HttpGet]
    public async Task<IActionResult> NovoAgendamentoEtapa2()
    {
        short idEstadoPaciente = Convert.ToInt16(TempData["idEstadoPaciente"]);
        int idCidadePaciente = Convert.ToInt32(TempData["idCidadePaciente"]);
        Etapa2ViewModel etapaAtual = new Etapa2ViewModel()
        {
            idInstituicao = TempData["idInstituicao"] is null ? null : Convert.ToInt32(TempData["idInstituicao"]),
            idPratica = TempData["idPratica"] is null ? null : Convert.ToInt16(TempData["idPratica"])
        };
        TempData.Keep();

        if (idEstadoPaciente == 0 || idCidadePaciente == 0)
            return RedirectToAction("NovoAgendamentoEtapa1");

        var instituicoesQuery = new ObterInstituicoesQuery(idCidadePaciente);
        var instituicoesResult = await _mediator.Send(instituicoesQuery);

        if (instituicoesResult.IsSuccess)
            ViewBag.Instituicoes = instituicoesResult.Value;

        return View(etapaAtual);
    }

    [HttpPost]
    public async Task<IActionResult> NovoAgendamentoEtapa2(int idInstituicao, short idPratica)
    {
        TempData["idInstituicao"] = idInstituicao.ToString();
        TempData["idPratica"] = idPratica.ToString();

        return Json(new { sucesso = true });
    }

    [HttpGet]
    public async Task<IActionResult> NovoAgendamentoEtapa3()
    {
        Etapa3ViewModel etapaAtual = new Etapa3ViewModel()
        {
            dataPrimeiroAtendimento = TempData["dataInicio"] is null ? null : Convert.ToDateTime(TempData["dataInicio"])
        };

        int idInstituicao = Convert.ToInt32(TempData["idInstituicao"]);
        short idPratica = Convert.ToInt16(TempData["idPratica"]);
        TempData.Keep();

        if (idInstituicao == 0 || idPratica == 0)
            return RedirectToAction("NovoAgendamentoEtapa2");

        var query = new ObterPraticaInstituicaoQuery(idPratica, idInstituicao);
        var result = await _mediator.Send(query);
        if (result.IsFailure)
            return Json(new { sucesso = false, mensagem = result.Error });

        ViewBag.DiaPermitidoParaAgendamento = result.Value.DiaPermitidoParaAgendamento;

        return View(etapaAtual);
    }

    [HttpPost]
    public async Task<IActionResult> NovoAgendamentoEtapa3(DateTime dataPrimeiroAtendimento)
    {
        TempData["dataPrimeiroAtendimento"] = dataPrimeiroAtendimento.ToString("yyyy-MM-dd");

        return Json(new { sucesso = true });
    }

    [HttpGet]
    public IActionResult NovoAgendamentoEtapa4()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> NovoAgendamentoEtapa4(
        string nomePaciente,
        string cpfPaciente,
        string telefonePaciente,
        DateTime dataNascimentoPaciente,
        byte generoPaciente,
        string emailPaciente,
        byte grauAnsiedadePaciente,
        string queixaPaciente)
    {
        int idInstituicao = Convert.ToInt32(TempData["idInstituicao"]);
        short idPratica = Convert.ToInt16(TempData["idPratica"]);
        short idEstadoPaciente = Convert.ToInt16(TempData["idEstadoPaciente"]);
        int idCidadePaciente = Convert.ToInt32(TempData["idCidadePaciente"]);
        DateTime? dataPrimeiroAtendimento = DateTime.Parse(TempData["dataPrimeiroAtendimento"]?.ToString() ?? DateTime.Now.ToString("yyyy-MM-dd"));
        TempData.Keep();

        if (idEstadoPaciente == 0 || idCidadePaciente == 0 || idInstituicao == 0 || idPratica == 0 || dataPrimeiroAtendimento is null)
            return RedirectToAction("NovoAgendamentoEtapa3");

        var command = new CriarAgendamentoCommand(
            idInstituicao,
            idPratica,
            nomePaciente,
            cpfPaciente,
            telefonePaciente,
            dataNascimentoPaciente,
            generoPaciente,
            emailPaciente,
            idEstadoPaciente,
            idCidadePaciente,
            grauAnsiedadePaciente,
            dataPrimeiroAtendimento.Value,
            queixaPaciente);

        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return Json(new { sucesso = false, mensagem = result.Error });

        return Json(new { sucesso = true, idAgendamento = result.Value });
    }

    [HttpGet]
    public IActionResult Sucesso()
    {
        return View();
    }
}
