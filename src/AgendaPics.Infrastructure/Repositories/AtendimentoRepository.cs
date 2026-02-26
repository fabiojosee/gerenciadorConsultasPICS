using AgendaPics.Domain.Entities;
using AgendaPics.Domain.Enums;
using AgendaPics.Domain.Interfaces;
using AgendaPics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AgendaPics.Infrastructure.Repositories;

public class AtendimentoRepository : Repository<Atendimento>, IAtendimentoRepository
{
    public AtendimentoRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Atendimento>> ObterPorAgendamentoAsync(int idAgendamento, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(a => a.IdAgendamento == idAgendamento)
            .OrderBy(a => a.DataAtendimento)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Atendimento>> ObterPorCpfPacienteAsync(string cpfPaciente, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Agendamento)
            .Where(a => a.Agendamento != null && a.Agendamento.CpfPaciente == cpfPaciente)
            .OrderByDescending(a => a.DataAtendimento)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Atendimento>> ObterPorInstituicaoAsync(int idInstituicao, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Agendamento)
            .Where(a => a.Agendamento != null && a.Agendamento.IdInstituicao == idInstituicao)
            .OrderByDescending(a => a.DataAtendimento)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Atendimento>> ObterPorStatusAsync(byte status, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(a => a.Status == status)
            .OrderByDescending(a => a.DataAtendimento)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Atendimento>> ObterNaoFinalizadosPorAgendamentoAsync(int idAgendamento, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(a => a.IdAgendamento == idAgendamento && a.Status == (byte)StatusAtendimento.Agendado)
            .ToListAsync(cancellationToken);
    }
}
