using gerenciadorConsultasPICS.Areas.Admin.Models;
using gerenciadorConsultasPICS.Data;
using gerenciadorConsultasPICS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace gerenciadorConsultasPICS.Repositories
{
    public class PraticaRepository : Repository<Pratica>, IPraticaRepository
    {
        public PraticaRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Pratica>> ObterPorInstituicao(int idInstituicao)
        {
            var query = from pratica in _context.Pratica
                        join praticaInstituicao in _context.PraticaInstituicao
                        on pratica.idPratica equals praticaInstituicao.idPratica
                        where praticaInstituicao.idInstituicao == idInstituicao
                        select new Pratica(pratica.idPratica, pratica.nome, pratica.descricao);

            return await query.ToListAsync();
        }
    }
}
