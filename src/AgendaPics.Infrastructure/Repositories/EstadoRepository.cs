using AgendaPics.Domain.Entities;
using AgendaPics.Domain.Interfaces;
using AgendaPics.Infrastructure.Data;

namespace AgendaPics.Infrastructure.Repositories;

public class EstadoRepository : Repository<Estado>, IEstadoRepository
{
    public EstadoRepository(AppDbContext context) : base(context)
    {
    }
}
