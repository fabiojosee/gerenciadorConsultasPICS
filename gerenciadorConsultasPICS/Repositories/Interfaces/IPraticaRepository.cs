using gerenciadorConsultasPICS.Areas.Admin.Models;

namespace gerenciadorConsultasPICS.Repositories.Interfaces
{
    public interface IPraticaRepository : IRepository<Pratica>
    {
        public Task<IEnumerable<Pratica>> ObterPorInstituicao(int idInstituicao);
    }
}
