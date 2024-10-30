using gerenciadorConsultasPICS.Areas.Usuario.Models;
using gerenciadorConsultasPICS.Data;
using gerenciadorConsultasPICS.Repositories.Interfaces;

namespace gerenciadorConsultasPICS.Repositories
{
    public class TermoConsentimentoRepository : Repository<TermoConsentimento>, ITermoConsentimentoRepository
    {
        public TermoConsentimentoRepository(AppDbContext context) : base(context) { }
    }
}
