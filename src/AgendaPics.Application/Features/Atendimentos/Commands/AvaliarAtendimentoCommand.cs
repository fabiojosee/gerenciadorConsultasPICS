using AgendaPics.Application.Common.Mediator;
using AgendaPics.Domain.Common;
using AgendaPics.Domain.Entities;
using AgendaPics.Domain.Enums;
using AgendaPics.Domain.Interfaces;

namespace AgendaPics.Application.Features.Atendimentos.Commands;

public record AvaliarAtendimentoCommand(
    int IdAtendimento,
    string Link,
    string Observacao) : IRequest<Result<int>>;

public class AvaliarAtendimentoCommandHandler : IRequestHandler<AvaliarAtendimentoCommand, Result<int>>
{
    private readonly IUnitOfWork _unitOfWork;

    public AvaliarAtendimentoCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> Handle(AvaliarAtendimentoCommand request, CancellationToken cancellationToken)
    {
        var atendimento = await _unitOfWork.Atendimentos.ObterPorIdAsync(request.IdAtendimento, cancellationToken);

        if (atendimento == null)
            return Result<int>.Failure("Atendimento não encontrado");

        if (atendimento.Status != (byte)StatusAtendimento.Finalizado)
            return Result<int>.Failure("Apenas atendimentos finalizados podem ser avaliados");

        if (string.IsNullOrWhiteSpace(request.Link))
            return Result<int>.Failure("Link da avaliação é obrigatório");

        var avaliacao = Avaliacao.Criar(
            request.IdAtendimento,
            DateTime.Now,
            request.Link,
            request.Observacao ?? string.Empty);

        await _unitOfWork.Avaliacoes.AdicionarAsync(avaliacao, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<int>.Success(avaliacao.IdAvaliacao);
    }
}
