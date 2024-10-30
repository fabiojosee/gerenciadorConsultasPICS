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
                        join instituicao in _context.Instituicao
                        on agendamento.idInstituicao equals instituicao.idInstituicao
                        where agendamento.cpfPaciente == cpfPaciente
                        orderby atendimento.dataAtendimento descending
                        select new MeusAtendimentosViewModel(
                            atendimento.idAtendimento,
                            pratica.nome,
                            cidade.nome,
                            estado.sigla,
                            atendimento.dataAtendimento,
                            ((StatusAtendimento)atendimento.status).ToString(),
                            atendimento.status,
                            agendamento.nomePaciente,
                            agendamento.telefonePaciente,
                            agendamento.dataNascimentoPaciente,
                            instituicao.horarioInicioAtendimento,
                            instituicao.horarioFimAtendimento,
                            instituicao.nome,
                            instituicao.cep
                        );

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Atendimento>> ObterPorAgendamento(int idAgendamento)
        {
            return await _context.Atendimento.Where(x => x.idAgendamento == idAgendamento).ToListAsync();
        }

        public async Task<IEnumerable<MeusAtendimentosViewModel>> ObterPorInstituicao(int idInstituicao)
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
                        where agendamento.idInstituicao == idInstituicao
                        orderby atendimento.dataAtendimento descending
                        select new MeusAtendimentosViewModel(
                            atendimento.idAtendimento,
                            pratica.nome,
                            cidade.nome,
                            estado.sigla,
                            atendimento.dataAtendimento,
                            ((StatusAtendimento)atendimento.status).ToString(),
                            atendimento.status,
                            agendamento.nomePaciente,
                            agendamento.telefonePaciente,
                            agendamento.dataNascimentoPaciente,
                            null,
                            null,
                            null,
                            null
                        );

            return await query.ToListAsync();
        }
    }
}
