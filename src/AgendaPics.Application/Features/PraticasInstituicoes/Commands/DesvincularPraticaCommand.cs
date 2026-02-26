using AgendaPics.Application.Common.Mediator;
using AgendaPics.Domain.Common;
using AgendaPics.Domain.Interfaces;

namespace AgendaPics.Application.Features.PraticasInstituicoes.Commands;

public record DesvincularPraticaCommand(short IdPratica, int IdInstituicao) : IRequest<Result<bool>>;

public class DesvincularPraticaCommandHandler : IRequestHandler<DesvincularPraticaCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DesvincularPraticaCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DesvincularPraticaCommand request, CancellationToken cancellationToken)
    {
        var agendamentos = await _unitOfWork.Agendamentos
            .ObterPorPraticaInstituicaoAsync(request.IdPratica, request.IdInstituicao, cancellationToken);

        if (agendamentos.Any())
            return Result<bool>.Failure("A prática não pode ser desvinculada, pois já existem agendamentos associados a ela.");

        await _unitOfWork.PraticasInstituicoes.RemoverAsync(request.IdPratica, request.IdInstituicao, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
