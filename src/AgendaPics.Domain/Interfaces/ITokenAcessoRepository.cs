using AgendaPics.Domain.Entities;

namespace AgendaPics.Domain.Interfaces;

public interface ITokenAcessoRepository : IRepository<TokenAcesso>
{
    Task<TokenAcesso?> ObterPorCpfETokenAsync(string cpf, string token, CancellationToken cancellationToken = default);
    Task<TokenAcesso?> ObterUltimoValidoPorCpfAsync(string cpf, CancellationToken cancellationToken = default);
}
