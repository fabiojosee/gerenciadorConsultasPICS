using gerenciadorConsultasPICS.Areas.Admin.Models;
using gerenciadorConsultasPICS.Data;
using gerenciadorConsultasPICS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace gerenciadorConsultasPICS.Repositories
{
    public class InstituicaoRepository : Repository<Instituicao>, IInstituicaoRepository
    {
        public InstituicaoRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Instituicao>> ObterPorEstado(short idEstado)
        {
            return await _context.Instituicao.Where(x => x.idEstado == idEstado).ToListAsync();
        }
    }
}
