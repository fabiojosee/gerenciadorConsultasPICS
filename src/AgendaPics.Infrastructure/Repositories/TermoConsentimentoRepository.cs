using AgendaPics.Domain.Entities;
using AgendaPics.Domain.Interfaces;
using AgendaPics.Infrastructure.Data;

namespace AgendaPics.Infrastructure.Repositories;

public class TermoConsentimentoRepository : Repository<TermoConsentimento>, ITermoConsentimentoRepository
{
    public TermoConsentimentoRepository(AppDbContext context) : base(context)
    {
    }
}
