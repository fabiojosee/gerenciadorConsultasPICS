using gerenciadorConsultasPICS.Areas.Admin.Enums;
using gerenciadorConsultasPICS.Areas.Usuario.Models;
using gerenciadorConsultasPICS.Areas.Usuario.ViewModels.Atendimento;
using gerenciadorConsultasPICS.Data;
using gerenciadorConsultasPICS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace gerenciadorConsultasPICS.Repositories
{
    public class PraticaInstituicaoRepository : Repository<PraticaInstituicao>, IPraticaInstituicaoRepository
    {
        public PraticaInstituicaoRepository(AppDbContext context) : base(context) { }

        public async Task<PraticaInstituicao> ObterPorPraticaInstituicao(int idInstituicao, int idPratica)
        {
            return await _context.PraticaInstituicao.Where(x => x.idInstituicao == idInstituicao && x.idPratica == idPratica).FirstAsync();
        }
    }
}
