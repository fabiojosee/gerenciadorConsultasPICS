using gerenciadorConsultasPICS.Helpers;
using gerenciadorConsultasPICS.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace gerenciadorConsultasPICS.Services
{
    public class TokenService : ITokenService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TokenService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public UsuarioInfo ObterInformacoesToken()
        {
            try
            {
                return new UsuarioInfo
                {
                    login = _httpContextAccessor.HttpContext.User.FindFirstValue("login"),
                    idPerfil = Convert.ToInt16(_httpContextAccessor.HttpContext.User.FindFirstValue("idPerfil")),
                    idInstituicao = _httpContextAccessor.HttpContext.User.FindFirst("idInstituicao").Value == "" ? null : Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("idInstituicao"))
                };
            }
            catch
            {
                throw new SecurityTokenException("Token inválido.");
            }
        }
    }
}
