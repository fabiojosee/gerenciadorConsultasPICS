using AgendaPics.Domain.Entities;

namespace AgendaPics.Domain.Interfaces;

public interface ICidadeRepository : IRepository<Cidade>
{
    Task<IEnumerable<Cidade>> ObterPorEstadoAsync(short idEstado, CancellationToken cancellationToken = default);
}
