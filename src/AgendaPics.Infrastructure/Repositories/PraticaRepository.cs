using AgendaPics.Domain.Entities;
using AgendaPics.Domain.Interfaces;
using AgendaPics.Infrastructure.Data;

namespace AgendaPics.Infrastructure.Repositories;

public class PraticaRepository : Repository<Pratica>, IPraticaRepository
{
    public PraticaRepository(AppDbContext context) : base(context)
    {
    }

    public async Task AtualizarAsync(Pratica pratica, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(pratica);
        await Task.CompletedTask;
    }

    public async Task RemoverAsync(short idPratica, CancellationToken cancellationToken = default)
    {
        var pratica = await ObterPorIdAsync(idPratica, cancellationToken);
        if (pratica != null)
        {
            _dbSet.Remove(pratica);
        }
    }
}
