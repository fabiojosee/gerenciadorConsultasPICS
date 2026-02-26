using AgendaPics.Application.Common.Mediator;
using AgendaPics.Domain.Common;
using AgendaPics.Domain.Enums;
using AgendaPics.Domain.Interfaces;

namespace AgendaPics.Application.Features.Atendimentos.Commands;

public record FinalizarAtendimentoCommand(int IdAtendimento, string? Observacao) : IRequest<Result>;

public class FinalizarAtendimentoCommandHandler : IRequestHandler<FinalizarAtendimentoCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public FinalizarAtendimentoCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(FinalizarAtendimentoCommand request, CancellationToken cancellationToken)
    {
        var atendimento = await _unitOfWork.Atendimentos.ObterPorIdAsync(request.IdAtendimento, cancellationToken);

        if (atendimento == null)
            return Result.Failure("Atendimento não encontrado");

        if (atendimento.Status == (byte)StatusAtendimento.Finalizado)
            return Result.Failure("Atendimento já está finalizado");

        if (atendimento.Status == (byte)StatusAtendimento.Cancelado)
            return Result.Failure("Não é possível finalizar um atendimento cancelado");

        atendimento.AlterarStatus(StatusAtendimento.Finalizado);

        if (!string.IsNullOrWhiteSpace(request.Observacao))
            atendimento.AdicionarObservacao(request.Observacao);

        _unitOfWork.Atendimentos.Atualizar(atendimento);

        #region Finalizar Agendamento

        var atendimentos = await _unitOfWork.Atendimentos.ObterNaoFinalizadosPorAgendamentoAsync(atendimento.IdAgendamento);
        if (atendimentos.Count() == 1)
        {
            var agendamento = await _unitOfWork.Agendamentos.ObterPorIdAsync(atendimento.IdAgendamento, cancellationToken);
            if (agendamento is not null)
            {
                agendamento.AlterarStatus(StatusAgendamento.Concluido);

                _unitOfWork.Agendamentos.Atualizar(agendamento);
            }
        }

        #endregion

        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }
}
