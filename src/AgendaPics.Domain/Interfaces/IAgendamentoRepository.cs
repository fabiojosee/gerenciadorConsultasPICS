using AgendaPics.Domain.Entities;

namespace AgendaPics.Domain.Interfaces;

public interface IAgendamentoRepository : IRepository<Agendamento>
{
    Task<IEnumerable<Agendamento>> ObterPorPraticaAsync(short idPratica, CancellationToken cancellationToken = default);
    Task<IEnumerable<Agendamento>> ObterPorPraticaInstituicaoAsync(short idPratica, int idInstituicao, CancellationToken cancellationToken = default);
    Task<IEnumerable<Agendamento>> ObterPorInstituicaoAsync(int idInstituicao, CancellationToken cancellationToken = default);
    Task<IEnumerable<Agendamento>> ObterPorPacienteAsync(short idPratica, string cpfPaciente, byte status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Agendamento>> ObterPorCpfAsync(string cpfPaciente, CancellationToken cancellationToken = default);
}
