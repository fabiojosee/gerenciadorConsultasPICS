using AgendaPics.Domain.Entities;

namespace AgendaPics.Domain.Interfaces;

public interface IPraticaRepository : IRepository<Pratica>
{
    Task AtualizarAsync(Pratica pratica, CancellationToken cancellationToken = default);
    Task RemoverAsync(short idPratica, CancellationToken cancellationToken = default);
}
