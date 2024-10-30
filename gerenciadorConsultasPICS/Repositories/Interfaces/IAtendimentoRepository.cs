using gerenciadorConsultasPICS.Areas.Usuario.Models;
using gerenciadorConsultasPICS.Areas.Usuario.ViewModels.Atendimento;

namespace gerenciadorConsultasPICS.Repositories.Interfaces
{
    public interface IAtendimentoRepository : IRepository<Atendimento>
    {
        public Task<IEnumerable<MeusAtendimentosViewModel>> ObterPorCpfPaciente(string cpfPaciente);
        public Task<IEnumerable<Atendimento>> ObterPorAgendamento(int idAgendamento);
        public Task<IEnumerable<MeusAtendimentosViewModel>> ObterPorInstituicao(int idInstituicao);
    }
}
