using AgendaPics.Application.Common.Mediator;
using AgendaPics.Domain.Common;
using AgendaPics.Domain.Interfaces;

namespace AgendaPics.Application.Features.Localidades.Queries;

public record ObterEstadosQuery : IRequest<Result<IEnumerable<EstadoDto>>>;

public record EstadoDto(short IdEstado, string Nome, string Sigla);

public class ObterEstadosQueryHandler : IRequestHandler<ObterEstadosQuery, Result<IEnumerable<EstadoDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ObterEstadosQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<EstadoDto>>> Handle(
        ObterEstadosQuery request,
        CancellationToken cancellationToken)
    {
        var estados = await _unitOfWork.Estados.ObterTodosAsync(cancellationToken);

        var dtos = estados
            .Select(e => new EstadoDto(e.IdEstado, e.Nome, e.Sigla))
            .OrderBy(e => e.Nome);

        return Result<IEnumerable<EstadoDto>>.Success(dtos);
    }
}
