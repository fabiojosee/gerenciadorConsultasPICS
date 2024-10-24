using gerenciadorConsultasPICS.Areas.Admin.Models;
using gerenciadorConsultasPICS.Data;
using gerenciadorConsultasPICS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace gerenciadorConsultasPICS.Repositories
{
    public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(AppDbContext context) : base(context) { }

        public async Task<Usuario?> ObterPorLogin(string login)
        {
            return await _context.Usuario.Where(x => x.login == login).FirstOrDefaultAsync();
        }

        public async Task<Usuario?> ObterPorInstituicao(int idInstituicao)
        {
            return await _context.Usuario.Where(x => x.idInstituicao == idInstituicao).FirstOrDefaultAsync();
        }
    }
}
