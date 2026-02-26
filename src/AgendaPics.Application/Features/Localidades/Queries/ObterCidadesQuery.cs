using AgendaPics.Application.Common.Mediator;
using AgendaPics.Domain.Common;
using AgendaPics.Domain.Interfaces;

namespace AgendaPics.Application.Features.Localidades.Queries;

public record ObterCidadesQuery(short IdEstado) : IRequest<Result<IEnumerable<CidadeDto>>>;

public record CidadeDto(int IdCidade, string Nome, short IdEstado);

public class ObterCidadesQueryHandler : IRequestHandler<ObterCidadesQuery, Result<IEnumerable<CidadeDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ObterCidadesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<CidadeDto>>> Handle(
        ObterCidadesQuery request,
        CancellationToken cancellationToken)
    {
        var cidades = await _unitOfWork.Cidades.ObterPorEstadoAsync(request.IdEstado, cancellationToken);

        var dtos = cidades
            .Select(c => new CidadeDto(c.IdCidade, c.Nome, c.IdEstado))
            .OrderBy(c => c.Nome);

        return Result<IEnumerable<CidadeDto>>.Success(dtos);
    }
}
