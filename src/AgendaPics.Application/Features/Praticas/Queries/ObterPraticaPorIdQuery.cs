using AgendaPics.Application.Common.Mediator;
using AgendaPics.Domain.Common;
using AgendaPics.Domain.Interfaces;

namespace AgendaPics.Application.Features.Praticas.Queries;

public record ObterPraticaPorIdQuery(short IdPratica) : IRequest<Result<PraticaDto>>;

public class ObterPraticaPorIdQueryHandler : IRequestHandler<ObterPraticaPorIdQuery, Result<PraticaDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ObterPraticaPorIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PraticaDto>> Handle(ObterPraticaPorIdQuery request, CancellationToken cancellationToken)
    {
        var pratica = await _unitOfWork.Praticas.ObterPorIdAsync(request.IdPratica, cancellationToken);
        if (pratica is null)
            return Result<PraticaDto>.Failure("Prática não encontrada.");

        return Result<PraticaDto>.Success(new PraticaDto(pratica.IdPratica, pratica.Nome, pratica.Descricao));
    }
}
