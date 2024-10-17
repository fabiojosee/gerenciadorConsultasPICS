using gerenciadorConsultasPICS.Areas.Admin.Models;
using gerenciadorConsultasPICS.Data;
using gerenciadorConsultasPICS.Repositories.Interfaces;

namespace gerenciadorConsultasPICS.Repositories
{
    public class EstadoRepository : Repository<Estado>, IEstadoRepository
    {
        public EstadoRepository(AppDbContext context) : base(context) { }

    }
}
