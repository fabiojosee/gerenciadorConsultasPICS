using gerenciadorConsultasPICS.Areas.Usuario.Models;

namespace gerenciadorConsultasPICS.Repositories.Interfaces
{
    public interface IPraticaInstituicaoRepository : IRepository<PraticaInstituicao>
    {
        public Task<PraticaInstituicao> ObterPorPraticaInstituicao(int idInstituicao, int idPratica);
    }
}
