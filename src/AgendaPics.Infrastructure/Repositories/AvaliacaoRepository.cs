using AgendaPics.Domain.Entities;
using AgendaPics.Domain.Interfaces;
using AgendaPics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AgendaPics.Infrastructure.Repositories;

public class AvaliacaoRepository : Repository<Avaliacao>, IAvaliacaoRepository
{
    public AvaliacaoRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Avaliacao>> ObterPorAtendimentoAsync(int idAtendimento, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(a => a.IdAtendimento == idAtendimento)
            .OrderByDescending(a => a.Data)
            .ToListAsync(cancellationToken);
    }
}
