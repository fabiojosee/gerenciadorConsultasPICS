using AgendaPics.Domain.Entities;

namespace AgendaPics.Domain.Interfaces;

public interface IInstituicaoRepository : IRepository<Instituicao>
{
    Task<IEnumerable<Instituicao>> ObterPorEstadoAsync(short idEstado, CancellationToken cancellationToken = default);
    Task<Instituicao?> ObterPorEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<Instituicao>> ObterPorCidadeAsync(int idCidade, CancellationToken cancellationToken = default);
    Task<Instituicao?> ObterPorCnpjAsync(string cnpj, CancellationToken cancellationToken = default);
}
