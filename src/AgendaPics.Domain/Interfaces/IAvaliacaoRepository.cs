using AgendaPics.Domain.Entities;

namespace AgendaPics.Domain.Interfaces;

public interface IAvaliacaoRepository : IRepository<Avaliacao>
{
    Task<IEnumerable<Avaliacao>> ObterPorAtendimentoAsync(int idAtendimento, CancellationToken cancellationToken = default);
}
