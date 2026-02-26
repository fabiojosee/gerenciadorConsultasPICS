using AgendaPics.Application.Common.Mediator;
using AgendaPics.Domain.Common;
using AgendaPics.Domain.Interfaces;

namespace AgendaPics.Application.Features.Atendimentos.Queries;

public record ObterAtendimentosPorInstituicaoQuery(int IdInstituicao) : IRequest<Result<IEnumerable<AtendimentoInstituicaoDto>>>;

public record AtendimentoInstituicaoDto(
    int IdAtendimento,
    int IdAgendamento,
    DateTime DataAtendimento,
    byte Status,
    string StatusDescricao,
    string NomePaciente,
    string CpfPaciente,
    string TelefonePaciente,
    string NomePratica,
    string CidadePaciente,
    string EstadoPaciente,
    DateTime DataNascimentoPaciente,
    TimeSpan HorarioInicioAtendimento,
    TimeSpan HorarioFimAtendimento);

public class ObterAtendimentosPorInstituicaoQueryHandler : IRequestHandler<ObterAtendimentosPorInstituicaoQuery, Result<IEnumerable<AtendimentoInstituicaoDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ObterAtendimentosPorInstituicaoQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<AtendimentoInstituicaoDto>>> Handle(
        ObterAtendimentosPorInstituicaoQuery request,
        CancellationToken cancellationToken)
    {
        var instituicao = await _unitOfWork.Instituicoes.ObterPorIdAsync(request.IdInstituicao, cancellationToken);
        if (instituicao == null)
            return Result<IEnumerable<AtendimentoInstituicaoDto>>.Failure("Instituição não encontrada");

        var atendimentos = await _unitOfWork.Atendimentos.ObterPorInstituicaoAsync(request.IdInstituicao, cancellationToken);

        var dtos = new List<AtendimentoInstituicaoDto>();

        foreach (var atendimento in atendimentos)
        {
            var agendamento = await _unitOfWork.Agendamentos.ObterPorIdAsync(atendimento.IdAgendamento, cancellationToken);
            if (agendamento == null) continue;

            var pratica = await _unitOfWork.Praticas.ObterPorIdAsync(agendamento.IdPratica, cancellationToken);
            var cidade = await _unitOfWork.Cidades.ObterPorIdAsync(agendamento.IdCidadePaciente, cancellationToken);
            var estado = await _unitOfWork.Estados.ObterPorIdAsync(agendamento.IdEstadoPaciente, cancellationToken);

            dtos.Add(new AtendimentoInstituicaoDto(
                atendimento.IdAtendimento,
                atendimento.IdAgendamento,
                atendimento.DataAtendimento,
                atendimento.Status,
                GetStatusDescricao(atendimento.Status),
                agendamento.NomePaciente,
                MascararCpf(agendamento.CpfPaciente),
                agendamento.TelefonePaciente,
                pratica?.Nome ?? "N/A",
                cidade?.Nome ?? "N/A",
                estado?.Nome ?? "N/A",
                agendamento.DataNascimentoPaciente,
                instituicao.HorarioInicioAtendimento,
                instituicao.HorarioFimAtendimento
            ));
        }

        return Result<IEnumerable<AtendimentoInstituicaoDto>>.Success(dtos.OrderByDescending(x => x.DataAtendimento));
    }

    private static string GetStatusDescricao(byte status)
    {
        return status switch
        {
            1 => "Agendado",
            2 => "Finalizado",
            3 => "Cancelado",
            _ => "Desconhecido"
        };
    }

    private static string MascararCpf(string cpf)
    {
        if (cpf.Length != 11) return cpf;
        return $"{cpf[..3]}.***.***.{cpf[9..]}";
    }
}
