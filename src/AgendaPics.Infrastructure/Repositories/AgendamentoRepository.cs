using AgendaPics.Domain.Entities;
using AgendaPics.Domain.Interfaces;
using AgendaPics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AgendaPics.Infrastructure.Repositories;

public class AgendamentoRepository : Repository<Agendamento>, IAgendamentoRepository
{
    public AgendamentoRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Agendamento>> ObterPorPraticaAsync(short idPratica, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(a => a.IdPratica == idPratica)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Agendamento>> ObterPorPraticaInstituicaoAsync(short idPratica, int idInstituicao, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(a => a.IdPratica == idPratica && a.IdInstituicao == idInstituicao)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Agendamento>> ObterPorInstituicaoAsync(int idInstituicao, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(a => a.IdInstituicao == idInstituicao)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Agendamento>> ObterPorPacienteAsync(short idPratica, string cpfPaciente, byte status, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(a => a.IdPratica == idPratica && a.CpfPaciente == cpfPaciente && a.Status == status)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Agendamento>> ObterPorCpfAsync(string cpfPaciente, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(a => a.CpfPaciente == cpfPaciente)
            .ToListAsync(cancellationToken);
    }
}
