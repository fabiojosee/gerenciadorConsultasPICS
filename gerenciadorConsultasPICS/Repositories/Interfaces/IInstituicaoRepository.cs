using gerenciadorConsultasPICS.Areas.Admin.Models;

namespace gerenciadorConsultasPICS.Repositories.Interfaces
{
    public interface IInstituicaoRepository : IRepository<Instituicao>
    {
        public Task<IEnumerable<Instituicao>> ObterPorEstado(short idEstado);
        public Task<Instituicao?> ObterPorEmail(string email);
    }
}
