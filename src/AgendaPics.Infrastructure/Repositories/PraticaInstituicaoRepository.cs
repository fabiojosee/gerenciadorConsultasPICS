using AgendaPics.Domain.Entities;
using AgendaPics.Domain.Interfaces;
using AgendaPics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AgendaPics.Infrastructure.Repositories;

public class PraticaInstituicaoRepository : Repository<PraticaInstituicao>, IPraticaInstituicaoRepository
{
    public PraticaInstituicaoRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<PraticaInstituicao?> ObterPorPraticaInstituicaoAsync(int idInstituicao, short idPratica, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(pi => pi.IdInstituicao == idInstituicao && pi.IdPratica == idPratica, cancellationToken);
    }

    public async Task<IEnumerable<PraticaInstituicao>> ObterPorInstituicaoAsync(int idInstituicao, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(pi => pi.IdInstituicao == idInstituicao)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<PraticaInstituicao>> ObterPorPraticaAsync(short idPratica, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(pi => pi.IdPratica == idPratica)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<PraticaInstituicao>> ObterInstituicoesVinculadasAsync(short idPratica, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(pi => pi.IdPratica == idPratica)
            .ToListAsync(cancellationToken);
    }

    public async Task AtualizarAsync(PraticaInstituicao praticaInstituicao, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(praticaInstituicao);
        await Task.CompletedTask;
    }

    public async Task RemoverAsync(short idPratica, int idInstituicao, CancellationToken cancellationToken = default)
    {
        var praticaInstituicao = await ObterPorPraticaInstituicaoAsync(idInstituicao, idPratica, cancellationToken);
        if (praticaInstituicao != null)
        {
            _dbSet.Remove(praticaInstituicao);
        }
    }
}
