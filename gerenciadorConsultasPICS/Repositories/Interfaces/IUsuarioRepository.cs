using gerenciadorConsultasPICS.Areas.Admin.Models;

namespace gerenciadorConsultasPICS.Repositories.Interfaces
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        public Task<Usuario?> ObterPorLogin(string login);
        public Task<Usuario?> ObterPorInstituicao(int idInstituicao);
    }
}
