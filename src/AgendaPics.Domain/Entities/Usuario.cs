using AgendaPics.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgendaPics.Domain.Entities;

public class Usuario : Entity<int>
{
    protected Usuario() { }

    public Usuario(int idUsuario, byte idPerfil, int? idInstituicao, string login, string senha, bool flPrimeiroAcesso)
    {
        IdUsuario = idUsuario;
        IdPerfil = idPerfil;
        IdInstituicao = idInstituicao;
        Login = login;
        Senha = senha;
        FlPrimeiroAcesso = flPrimeiroAcesso;
    }

    [Key]
    public int IdUsuario { get; private set; }
    public byte IdPerfil { get; private set; }
    [ForeignKey(nameof(Instituicao))]
    public int? IdInstituicao { get; private set; }
    public string Login { get; private set; } = null!;
    public string Senha { get; private set; } = null!;
    public bool FlPrimeiroAcesso { get; private set; }

    public virtual Instituicao? Instituicao { get; private set; }

    public void AlterarSenha(string senha)
    {
        Senha = senha;
    }

    public void AlterarFlagPrimeiroAcesso(bool flPrimeiroAcesso)
    {
        FlPrimeiroAcesso = flPrimeiroAcesso;
    }

    public Perfil GetPerfilEnum() => (Perfil)IdPerfil;
    public bool IsAdmin => IdPerfil == (byte)Perfil.Admin;
    public bool IsInstituicao => IdPerfil == (byte)Perfil.Instituicao;

    public static Usuario Criar(byte idPerfil, int? idInstituicao, string login, string senha)
    {
        return new Usuario
        {
            IdPerfil = idPerfil,
            IdInstituicao = idInstituicao,
            Login = login,
            Senha = senha,
            FlPrimeiroAcesso = true
        };
    }
}
