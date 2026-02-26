using AgendaPics.Domain.Entities;
using AgendaPics.Domain.Interfaces;
using AgendaPics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AgendaPics.Infrastructure.Repositories;

public class CidadeRepository : Repository<Cidade>, ICidadeRepository
{
    public CidadeRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Cidade>> ObterPorEstadoAsync(short idEstado, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.IdEstado == idEstado)
            .OrderBy(c => c.Nome)
            .ToListAsync(cancellationToken);
    }
}
