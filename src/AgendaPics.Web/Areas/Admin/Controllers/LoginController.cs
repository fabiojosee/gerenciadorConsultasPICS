using AgendaPics.Application.Common.Interfaces;
using AgendaPics.Application.Common.Mediator;
using AgendaPics.Application.Features.Autenticacao.Commands;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AgendaPics.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class LoginController : Controller
{
    private readonly ILogger<LoginController> _logger;
    private readonly IMediator _mediator;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    public LoginController(
        ILogger<LoginController> logger,
        IMediator mediator,
        ITokenService tokenService,
        IConfiguration configuration)
    {
        _logger = logger;
        _mediator = mediator;
        _tokenService = tokenService;
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult Login()
    {
        ViewBag.FormularioNovaInstituicao = _configuration["Formularios:NovaInstituicao"];
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string login, string senha)
    {
        var command = new LoginCommand(login, senha);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return Json(new { sucesso = false, mensagem = result.Error });

        TempData["FlPrimeiroAcesso"] = result.Value.FlPrimeiroAcesso.ToString();

        var loginResult = result.Value;

        var claims = new List<Claim>
        {
            new("login", loginResult.Login),
            new("idPerfil", loginResult.IdPerfil.ToString()),
            new("idInstituicao", loginResult.IdInstituicao?.ToString() ?? "")
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity));

        return Json(new { sucesso = true, idPerfil = loginResult.IdPerfil, primeiroAcesso = loginResult.FlPrimeiroAcesso });
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult RecuperacaoSenha()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> EnviarCodigoRecuperacaoSenha(string email)
    {
        var command = new EnviarCodigoRecuperacaoCommand(email);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return Json(new { sucesso = false, mensagem = result.Error });

        TempData["CodigoRecuperacaoSenha"] = result.Value.Codigo;
        TempData["InstituicaoAtual"] = result.Value.IdInstituicao.ToString();

        return Json(new { sucesso = true });
    }

    [HttpGet]
    public IActionResult RecuperacaoSenhaCodigo()
    {
        return View();
    }

    [HttpPut]
    public async Task<IActionResult> AlterarSenha(string codigo, string senha, string confirmacaoSenha)
    {
        if (TempData["CodigoRecuperacaoSenha"] is null)
            return Json(new { sucesso = false, mensagem = "Código expirado." });

        var codigoRecuperacaoSenha = TempData["CodigoRecuperacaoSenha"]?.ToString();
        var idInstituicao = Convert.ToInt32(TempData["InstituicaoAtual"]);
        TempData.Keep();

        if (codigo != codigoRecuperacaoSenha)
            return Json(new { sucesso = false, mensagem = "Código inválido." });

        var command = new AlterarSenhaCommand(idInstituicao, null, senha, confirmacaoSenha, false);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return Json(new { sucesso = false, mensagem = result.Error });

        return Json(new { sucesso = true });
    }

    [HttpGet]
    public IActionResult RecuperacaoSenhaSucesso()
    {
        return View();
    }

    [HttpGet]
    [Authorize(Policy = "ApenasAdmin")]
    public IActionResult AreaAdministrativa()
    {
        var usuarioInfo = _tokenService.ObterInformacoesToken();
        if (usuarioInfo == null)
            return RedirectToAction("Login");

        var flPrimeiroAcesso = Convert.ToBoolean(TempData["FlPrimeiroAcesso"]);
        if (flPrimeiroAcesso)
        {
            TempData.Remove("FlPrimeiroAcesso");
            return RedirectToAction("AlteracaoSenha");
        }

        return View();
    }

    [HttpGet]
    [Authorize(Policy = "ApenasInstituicao")]
    public IActionResult AreaAdministrativaInstituicao()
    {
        var usuarioInfo = _tokenService.ObterInformacoesToken();

        if (usuarioInfo == null)
            return RedirectToAction("Login");

        var flPrimeiroAcesso = Convert.ToBoolean(TempData["FlPrimeiroAcesso"]);
        if (flPrimeiroAcesso)
        {
            TempData.Remove("FlPrimeiroAcesso");
            return RedirectToAction("AlteracaoSenha");
        }

        ViewBag.FormularioNovaPratica = _configuration["Formularios:NovaPratica"];
        return View();
    }

    [HttpGet]
    [Authorize]
    public IActionResult AlteracaoSenha()
    {
        return View();
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> AlterarSenhaPrimeiroAcesso(string senha, string confirmacaoSenha)
    {
        var usuarioInfo = _tokenService.ObterInformacoesToken();

        if (usuarioInfo == null)
            return Json(new { sucesso = false, mensagem = "Usuário não autenticado." });

        AlterarSenhaCommand command;
        if (usuarioInfo.IdInstituicao.HasValue)
            command = new AlterarSenhaCommand(usuarioInfo.IdInstituicao, null, senha, confirmacaoSenha, true);
        else
            command = new AlterarSenhaCommand(null, usuarioInfo.Login, senha, confirmacaoSenha, true);

        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return Json(new { sucesso = false, mensagem = result.Error });

        return Json(new { sucesso = true });
    }
}
