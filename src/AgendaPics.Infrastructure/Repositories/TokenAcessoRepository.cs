using AgendaPics.Domain.Entities;
using AgendaPics.Domain.Interfaces;
using AgendaPics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AgendaPics.Infrastructure.Repositories;

public class TokenAcessoRepository : Repository<TokenAcesso>, ITokenAcessoRepository
{
    public TokenAcessoRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<TokenAcesso?> ObterPorCpfETokenAsync(string cpf, string token, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(t => t.Cpf == cpf && t.Token == token && !t.Utilizado && t.DataExpiracao > DateTime.Now, cancellationToken);
    }

    public async Task<TokenAcesso?> ObterUltimoValidoPorCpfAsync(string cpf, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.Cpf == cpf && !t.Utilizado && t.DataExpiracao > DateTime.Now)
            .OrderByDescending(t => t.DataExpiracao)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
