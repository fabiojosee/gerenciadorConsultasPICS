using AgendaPics.Domain.Entities;

namespace AgendaPics.Domain.Interfaces;

public interface IPraticaInstituicaoRepository : IRepository<PraticaInstituicao>
{
    Task<PraticaInstituicao?> ObterPorPraticaInstituicaoAsync(int idInstituicao, short idPratica, CancellationToken cancellationToken = default);
    Task<IEnumerable<PraticaInstituicao>> ObterPorInstituicaoAsync(int idInstituicao, CancellationToken cancellationToken = default);
    Task<IEnumerable<PraticaInstituicao>> ObterPorPraticaAsync(short idPratica, CancellationToken cancellationToken = default);
    Task<IEnumerable<PraticaInstituicao>> ObterInstituicoesVinculadasAsync(short idPratica, CancellationToken cancellationToken = default);
    Task AtualizarAsync(PraticaInstituicao praticaInstituicao, CancellationToken cancellationToken = default);
    Task RemoverAsync(short idPratica, int idInstituicao, CancellationToken cancellationToken = default);
}
