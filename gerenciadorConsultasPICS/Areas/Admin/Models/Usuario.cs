using System.ComponentModel.DataAnnotations;

namespace gerenciadorConsultasPICS.Areas.Admin.Models
{
    public class Usuario
    {
        protected Usuario() { }

        public Usuario(int idUsuario, byte idPerfil, int? idInstituicao, string login, string senha)
        {
            this.idUsuario = idUsuario;
            this.idPerfil = idPerfil;
            this.idInstituicao = idInstituicao;
            this.login = login;
            this.senha = senha;
        }

        [Key]
        public int idUsuario { get; private set; }
        public byte idPerfil { get; private set; }
        public int? idInstituicao { get; private set; }
        public string login { get; private set; }
        public string senha { get; private set; }

        public void AlterarSenha(string senha) => this.senha = senha;

        public static class UsuarioFactory
        {
            public static Usuario CriarUsuario(byte idPerfil, int? idInstituicao, string login, string senha)
            {
                return new Usuario()
                {
                    idPerfil = idPerfil,
                    idInstituicao = idInstituicao,
                    login = login,
                    senha = senha
                };
            }
        }
    }
}
