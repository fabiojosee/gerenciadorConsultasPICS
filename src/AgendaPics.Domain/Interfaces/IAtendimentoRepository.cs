using AgendaPics.Domain.Entities;

namespace AgendaPics.Domain.Interfaces;

public interface IAtendimentoRepository : IRepository<Atendimento>
{
    Task<IEnumerable<Atendimento>> ObterPorAgendamentoAsync(int idAgendamento, CancellationToken cancellationToken = default);
    Task<IEnumerable<Atendimento>> ObterPorCpfPacienteAsync(string cpfPaciente, CancellationToken cancellationToken = default);
    Task<IEnumerable<Atendimento>> ObterPorInstituicaoAsync(int idInstituicao, CancellationToken cancellationToken = default);
    Task<IEnumerable<Atendimento>> ObterPorStatusAsync(byte status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Atendimento>> ObterNaoFinalizadosPorAgendamentoAsync(int idAgendamento, CancellationToken cancellationToken = default);
}
