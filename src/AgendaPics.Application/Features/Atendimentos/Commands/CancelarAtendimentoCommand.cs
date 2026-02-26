using AgendaPics.Application.Common.Mediator;
using AgendaPics.Domain.Common;
using AgendaPics.Domain.Enums;
using AgendaPics.Domain.Interfaces;

namespace AgendaPics.Application.Features.Atendimentos.Commands;

public record CancelarAtendimentoCommand(int IdAtendimento, string? Observacao) : IRequest<Result>;

public class CancelarAtendimentoCommandHandler : IRequestHandler<CancelarAtendimentoCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public CancelarAtendimentoCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CancelarAtendimentoCommand request, CancellationToken cancellationToken)
    {
        var atendimento = await _unitOfWork.Atendimentos.ObterPorIdAsync(request.IdAtendimento, cancellationToken);

        if (atendimento == null)
            return Result.Failure("Atendimento não encontrado");

        if (atendimento.Status == (byte)StatusAtendimento.Cancelado)
            return Result.Failure("Atendimento já está cancelado");

        if (atendimento.Status == (byte)StatusAtendimento.Finalizado)
            return Result.Failure("Não é possível cancelar um atendimento finalizado");

        var atendimentos = await _unitOfWork.Atendimentos.ObterNaoFinalizadosPorAgendamentoAsync(atendimento.IdAgendamento);

        foreach (var atendimentoAgendado in atendimentos)
        {
            atendimentoAgendado.AlterarStatus(StatusAtendimento.Cancelado);

            if (!string.IsNullOrWhiteSpace(request.Observacao))
                atendimentoAgendado.AdicionarObservacao(request.Observacao);

            _unitOfWork.Atendimentos.Atualizar(atendimentoAgendado);
        }

        #region Cancelar Agendamento

        var agendamento = await _unitOfWork.Agendamentos.ObterPorIdAsync(atendimento.IdAgendamento, cancellationToken);
        if (agendamento is not null)
        {
            agendamento.AlterarStatus(StatusAgendamento.Cancelado);

            _unitOfWork.Agendamentos.Atualizar(agendamento);
        }

        #endregion

        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }
}
