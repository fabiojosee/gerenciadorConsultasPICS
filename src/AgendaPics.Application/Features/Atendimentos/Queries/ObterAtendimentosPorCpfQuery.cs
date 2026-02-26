using AgendaPics.Application.Common.Interfaces;
using AgendaPics.Application.Common.Mediator;
using AgendaPics.Domain.Common;
using AgendaPics.Domain.Interfaces;
using AgendaPics.Domain.ValueObjects;

namespace AgendaPics.Application.Features.Atendimentos.Queries;

public record ObterAtendimentosPorCpfQuery(string Cpf, string Token, bool ReutilizarToken = false) : IRequest<Result<IEnumerable<AtendimentoDto>>>;

public record AtendimentoDto(
    int IdAtendimento,
    int IdAgendamento,
    DateTime DataAtendimento,
    byte Status,
    string StatusDescricao,
    string QueixaPaciente,
    string? Observacao,
    string NomeInstituicao,
    string NomePratica,
    string CidadePaciente,
    string EstadoPaciente,
    TimeSpan HorarioInicioAtendimento,
    TimeSpan HorarioFimAtendimento,
    string CepInstituicao);

public class ObterAtendimentosPorCpfQueryHandler : IRequestHandler<ObterAtendimentosPorCpfQuery, Result<IEnumerable<AtendimentoDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAccessTokenService _accessTokenService;

    public ObterAtendimentosPorCpfQueryHandler(IUnitOfWork unitOfWork, IAccessTokenService accessTokenService)
    {
        _unitOfWork = unitOfWork;
        _accessTokenService = accessTokenService;
    }

    public async Task<Result<IEnumerable<AtendimentoDto>>> Handle(
        ObterAtendimentosPorCpfQuery request,
        CancellationToken cancellationToken)
    {
        if (!CPF.IsValid(request.Cpf))
            return Result<IEnumerable<AtendimentoDto>>.Failure("CPF inválido");

        if (!request.ReutilizarToken)
        {
            var tokenValido = await _accessTokenService.ValidarTokenAsync(request.Cpf, request.Token);
            if (!tokenValido)
                return Result<IEnumerable<AtendimentoDto>>.Failure("Token inválido ou expirado");
        }

        var cpfLimpo = new string(request.Cpf.Where(char.IsDigit).ToArray());
        var atendimentos = await _unitOfWork.Atendimentos.ObterPorCpfPacienteAsync(cpfLimpo, cancellationToken);

        var dtos = new List<AtendimentoDto>();

        foreach (var atendimento in atendimentos)
        {
            var agendamento = await _unitOfWork.Agendamentos.ObterPorIdAsync(atendimento.IdAgendamento, cancellationToken);
            if (agendamento == null) continue;

            var instituicao = await _unitOfWork.Instituicoes.ObterPorIdAsync(agendamento.IdInstituicao, cancellationToken);
            var pratica = await _unitOfWork.Praticas.ObterPorIdAsync(agendamento.IdPratica, cancellationToken);
            var cidade = await _unitOfWork.Cidades.ObterPorIdAsync(agendamento.IdCidadePaciente, cancellationToken);
            var estado = await _unitOfWork.Estados.ObterPorIdAsync(agendamento.IdEstadoPaciente, cancellationToken);

            dtos.Add(new AtendimentoDto(
                atendimento.IdAtendimento,
                atendimento.IdAgendamento,
                atendimento.DataAtendimento,
                atendimento.Status,
                GetStatusDescricao(atendimento.Status),
                atendimento.QueixaPaciente,
                atendimento.Observacao,
                instituicao?.Nome ?? "N/A",
                pratica?.Nome ?? "N/A",
                cidade?.Nome ?? "N/A",
                estado?.Nome ?? "N/A",
                instituicao?.HorarioInicioAtendimento ?? TimeSpan.Zero,
                instituicao?.HorarioFimAtendimento ?? TimeSpan.Zero,
                instituicao?.Cep ?? "N/A"
            ));
        }

        return Result<IEnumerable<AtendimentoDto>>.Success(dtos.OrderByDescending(x => x.DataAtendimento));
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
}
