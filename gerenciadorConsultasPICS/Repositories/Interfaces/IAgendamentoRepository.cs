using gerenciadorConsultasPICS.Areas.Usuario.Models;

namespace gerenciadorConsultasPICS.Repositories.Interfaces
{
    public interface IAgendamentoRepository : IRepository<Agendamento>
    {
        public Task<IEnumerable<Agendamento>> ObterPorPratica(short idPratica);
        public Task<IEnumerable<Agendamento>> ObterPorPraticaInstituicao(short idPratica, int idInstituicao);
        public Task<IEnumerable<Agendamento>> ObterPorInstituicao(int idInstituicao);
    }
}
