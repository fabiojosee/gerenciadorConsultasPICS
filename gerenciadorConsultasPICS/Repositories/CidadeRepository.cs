using gerenciadorConsultasPICS.Areas.Admin.Models;
using gerenciadorConsultasPICS.Data;
using gerenciadorConsultasPICS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace gerenciadorConsultasPICS.Repositories
{
    public class CidadeRepository : Repository<Cidade>, ICidadeRepository
    {
        public CidadeRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Cidade>> ObterPorEstado(short idEstado)
        {
            return await _context.Cidade.Where(x => x.idEstado == idEstado).ToListAsync();
        }
    }
}
