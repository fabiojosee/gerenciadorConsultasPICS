using AgendaPics.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace AgendaPics.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TokenService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public UsuarioInfo? ObterInformacoesToken()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        if (user?.Identity?.IsAuthenticated != true)
            return null;

        var login = user.FindFirst("login")?.Value;
        var idPerfilClaim = user.FindFirst("idPerfil")?.Value;
        var idInstituicaoClaim = user.FindFirst("idInstituicao")?.Value;

        if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(idPerfilClaim))
            return null;

        if (!short.TryParse(idPerfilClaim, out var idPerfil))
            return null;

        int? idInstituicao = null;
        if (!string.IsNullOrEmpty(idInstituicaoClaim) && int.TryParse(idInstituicaoClaim, out var parsedIdInstituicao))
            idInstituicao = parsedIdInstituicao;

        return new UsuarioInfo(login, idPerfil, idInstituicao);
    }
}
