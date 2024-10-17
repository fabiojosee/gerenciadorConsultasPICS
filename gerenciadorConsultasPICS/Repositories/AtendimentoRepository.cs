using gerenciadorConsultasPICS.Areas.Admin.Enums;
using gerenciadorConsultasPICS.Areas.Usuario.Models;
using gerenciadorConsultasPICS.Areas.Usuario.ViewModels.Atendimento;
using gerenciadorConsultasPICS.Data;
using gerenciadorConsultasPICS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace gerenciadorConsultasPICS.Repositories
{
    public class AtendimentoRepository : Repository<Atendimento>, IAtendimentoRepository
    {
        public AtendimentoRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<MeusAtendimentosViewModel>> ObterPorCpfPaciente(string cpfPaciente)
        {
            var query = from atendimento in _context.Atendimento
                        join agendamento in _context.Agendamento
                        on atendimento.idAgendamento equals agendamento.idAgendamento
                        join pratica in _context.Pratica
                        on agendamento.idPratica equals pratica.idPratica
                        join estado in _context.Estado
                        on agendamento.idEstadoPaciente equals estado.idEstado
                        join cidade in _context.Cidade
                        on agendamento.idCidadePaciente equals cidade.idCidade
                        where agendamento.cpfPaciente == cpfPaciente
                        orderby atendimento.dataAtendimento descending
                        select new MeusAtendimentosViewModel
                        {
                            idAtendimento = atendimento.idAtendimento,
                            nomePratica = pratica.nome,
                            cidadePaciente = cidade.nome,
                            estadoPaciente = estado.sigla,
                            dataAtendimento = atendimento.dataAtendimento,
                            statusAtendimento = ((StatusAtendimento)atendimento.status).ToString(),
                            status = atendimento.status
                        };

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Atendimento>> ObterPorAgendamento(int idAgendamento)
        {
            return await _context.Atendimento.Where(x => x.idAgendamento == idAgendamento).ToListAsync();
        }
    }
}
