using AgendaPics.Domain.Entities;
using AgendaPics.Domain.Interfaces;
using AgendaPics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AgendaPics.Infrastructure.Repositories;

public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Usuario?> ObterPorLoginAsync(string login, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Login == login, cancellationToken);
    }

    public async Task<Usuario?> ObterPorInstituicaoAsync(int idInstituicao, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.IdInstituicao == idInstituicao, cancellationToken);
    }
}
