using AgendaPics.Domain.Entities;
using AgendaPics.Domain.Interfaces;
using AgendaPics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AgendaPics.Infrastructure.Repositories;

public class InstituicaoRepository : Repository<Instituicao>, IInstituicaoRepository
{
    public InstituicaoRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Instituicao>> ObterPorEstadoAsync(short idEstado, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(i => i.Cidade)
            .Where(i => i.Cidade != null && i.Cidade.IdEstado == idEstado)
            .OrderBy(i => i.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<Instituicao?> ObterPorEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(i => i.Email == email, cancellationToken);
    }

    public async Task<IEnumerable<Instituicao>> ObterPorCidadeAsync(int idCidade, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(i => i.IdCidade == idCidade)
            .OrderBy(i => i.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<Instituicao?> ObterPorCnpjAsync(string cnpj, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(i => i.Cnpj == cnpj, cancellationToken);
    }
}
