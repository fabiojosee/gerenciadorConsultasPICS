using AgendaPics.Domain.Entities;

namespace AgendaPics.Domain.Interfaces;

public interface IUsuarioRepository : IRepository<Usuario>
{
    Task<Usuario?> ObterPorLoginAsync(string login, CancellationToken cancellationToken = default);
    Task<Usuario?> ObterPorInstituicaoAsync(int idInstituicao, CancellationToken cancellationToken = default);
}
