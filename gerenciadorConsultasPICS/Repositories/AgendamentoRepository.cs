using gerenciadorConsultasPICS.Areas.Usuario.Models;
using gerenciadorConsultasPICS.Data;
using gerenciadorConsultasPICS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace gerenciadorConsultasPICS.Repositories
{
    public class AgendamentoRepository : Repository<Agendamento>, IAgendamentoRepository
    {
        public AgendamentoRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Agendamento>> ObterPorPratica(short idPratica)
        {
            return await _context.Agendamento.Where(x => x.idPratica == idPratica).ToListAsync();
        }

        public async Task<IEnumerable<Agendamento>> ObterPorPraticaInstituicao(short idPratica, int idInstituicao)
        {
            return await _context.Agendamento.Where(x => x.idPratica == idPratica && x.idInstituicao == idInstituicao).ToListAsync();
        }

        public async Task<IEnumerable<Agendamento>> ObterPorInstituicao(int idInstituicao)
        {
            return await _context.Agendamento.Where(x => x.idInstituicao == idInstituicao).ToListAsync();
        }
    }
}
