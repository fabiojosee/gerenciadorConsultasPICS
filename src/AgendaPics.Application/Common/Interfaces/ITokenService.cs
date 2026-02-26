namespace AgendaPics.Application.Common.Interfaces;

public interface ITokenService
{
    UsuarioInfo? ObterInformacoesToken();
}

public record UsuarioInfo(string Login, short IdPerfil, int? IdInstituicao);
