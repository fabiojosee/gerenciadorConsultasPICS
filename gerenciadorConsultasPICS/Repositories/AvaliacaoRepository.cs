using gerenciadorConsultasPICS.Areas.Usuario.Models;
using gerenciadorConsultasPICS.Data;
using gerenciadorConsultasPICS.Repositories.Interfaces;

namespace gerenciadorConsultasPICS.Repositories
{
    public class AvaliacaoRepository : Repository<Avaliacao>, IAvaliacaoRepository
    {
        public AvaliacaoRepository(AppDbContext context) : base(context) { }
    }
}
