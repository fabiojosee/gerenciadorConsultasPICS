using gerenciadorConsultasPICS.Areas.Admin.Models;
using gerenciadorConsultasPICS.Areas.Admin.ViewModels.Pratica;

namespace gerenciadorConsultasPICS.Repositories.Interfaces
{
    public interface IPraticaInstituicaoRepository : IRepository<PraticaInstituicao>
    {
        public Task<PraticaInstituicao> ObterPorPraticaInstituicao(int idInstituicao, short idPratica);
        public Task<IEnumerable<MinhasPraticasViewModel>> ObterMinhasPraticas(int idInstituicao);
        public Task<IEnumerable<PraticaInstituicao>> ObterInstituicoesVinculadas(short idPratica);
    }
}
