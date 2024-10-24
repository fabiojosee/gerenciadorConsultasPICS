using gerenciadorConsultasPICS.Helpers;
using gerenciadorConsultasPICS.Repositories.Interfaces;
using gerenciadorConsultasPICS.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace gerenciadorConsultasPICS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IInstituicaoRepository _instituicaoRepository;
        private readonly IEmailService _emailService;

        public LoginController(
            ILogger<LoginController> logger,
            IUsuarioRepository usuarioRepository,
            IInstituicaoRepository instituicaoRepository,
            IEmailService emailService)
        {
            _logger = logger;
            _usuarioRepository = usuarioRepository;
            _instituicaoRepository = instituicaoRepository;
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string login, string senha)
        {
            var usuario = await _usuarioRepository.ObterPorLogin(login);
            if (usuario is null)
                return Json(new { sucesso = false, mensagem = "Usuário não encontrado." });

            if (HashHelper.Criptografar(senha) != usuario.senha)
                return Json(new { sucesso = false, mensagem = "Senha inválida." });

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, login)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                          new ClaimsPrincipal(claimsIdentity));

            return Json(new { sucesso = true, usuario.idPerfil });
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> RecuperacaoSenha()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EnviarCodigoRecuperacaoSenha(string email)
        {
            var instituicao = await _instituicaoRepository.ObterPorEmail(email);
            if (instituicao is null)
                return Json(new { sucesso = false, mensagem = "O e-mail informado não está cadastrado no sistema." });

            string codigo = EmailHelper.GerarCodigoAleatorio(6);

            TempData["CodigoRecuperacaoSenha"] = codigo;
            TempData["InstituicaoAtual"] = instituicao.idInstituicao;

            var mensagem = EmailHelper.GerarTemplateEmail("Recuperação de senha", new List<string>() { $"O código para recuperação de senha é: {codigo}." });

            await _emailService.EnviarEmailAsync(email, "Recuperação de senha", mensagem);

            return Json(new { sucesso = true });
        }

        [HttpGet]
        public async Task<IActionResult> RecuperacaoSenhaCodigo()
        {
            return View();
        }

        [HttpPut]
        public async Task<IActionResult> AlterarSenha(string codigo, string senha, string confirmacaoSenha)
        {
            if (senha != confirmacaoSenha)
                return Json(new { sucesso = false, mensagem = "As senhas são divergentes." });

            if (TempData["CodigoRecuperacaoSenha"] is null)
                return Json(new { sucesso = false, mensagem = "Código expirado." });

            var codigoRecuperacaoSenha = TempData["CodigoRecuperacaoSenha"].ToString();
            var idInstituicao = Convert.ToInt32(TempData["InstituicaoAtual"]);
            TempData.Keep();

            if (codigo != codigoRecuperacaoSenha)
                return Json(new { sucesso = false, mensagem = "Código inválido." });

            var usuario = await _usuarioRepository.ObterPorInstituicao(idInstituicao);

            usuario.AlterarSenha(HashHelper.Criptografar(senha));

            await _usuarioRepository.AtualizarAsync(usuario);

            return Json(new { sucesso = true });
        }

        [HttpGet]
        public IActionResult RecuperacaoSenhaSucesso()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AreaAdministrativa()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AreaAdministrativaInstituicao()
        {
            return View();
        }
    }
}
