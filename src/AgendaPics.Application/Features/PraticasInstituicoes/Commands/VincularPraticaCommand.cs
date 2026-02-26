using AgendaPics.Application.Common.Mediator;
using AgendaPics.Domain.Common;
using AgendaPics.Domain.Entities;
using AgendaPics.Domain.Interfaces;

namespace AgendaPics.Application.Features.PraticasInstituicoes.Commands;

public record VincularPraticaCommand(
    short IdPratica,
    int IdInstituicao,
    byte Periodicidade,
    short QtdSessoes,
    byte DiaPermitidoParaAgendamento) : IRequest<Result<bool>>;

public class VincularPraticaCommandHandler : IRequestHandler<VincularPraticaCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public VincularPraticaCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(VincularPraticaCommand request, CancellationToken cancellationToken)
    {
        if (request.IdInstituicao == 0)
            return Result<bool>.Failure("A seleção de instituição é obrigatória.");

        if (request.IdPratica == 0)
            return Result<bool>.Failure("A seleção de prática é obrigatória.");

        if (request.Periodicidade == 0)
            return Result<bool>.Failure("O campo periodicidade é obrigatório.");

        if (request.QtdSessoes == 0)
            return Result<bool>.Failure("O campo quantidade de sessões é obrigatório.");

        var praticaInstituicao = await _unitOfWork.PraticasInstituicoes
            .ObterPorPraticaInstituicaoAsync(request.IdInstituicao, request.IdPratica, cancellationToken);

        if (praticaInstituicao is not null)
            return Result<bool>.Failure("A prática selecionada já está vinculada à instituição.");

        var novoVinculo = PraticaInstituicao.Criar(
            request.IdPratica,
            request.IdInstituicao,
            request.Periodicidade,
            request.QtdSessoes,
            request.DiaPermitidoParaAgendamento);

        await _unitOfWork.PraticasInstituicoes.AdicionarAsync(novoVinculo, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
