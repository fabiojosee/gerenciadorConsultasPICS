using gerenciadorConsultasPICS.Areas.Admin.Models;

namespace gerenciadorConsultasPICS.Repositories.Interfaces
{
    public interface ICidadeRepository : IRepository<Cidade>
    {
        public Task<IEnumerable<Cidade>> ObterPorEstado(short idEstado);
    }
}
