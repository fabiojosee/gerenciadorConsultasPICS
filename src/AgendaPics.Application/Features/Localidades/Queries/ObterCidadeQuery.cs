using AgendaPics.Application.Common.Mediator;
using AgendaPics.Domain.Common;
using AgendaPics.Domain.Interfaces;

namespace AgendaPics.Application.Features.Localidades.Queries;

public record ObterCidadeQuery(int IdCidade) : IRequest<Result<CidadeDto>>;

public class ObterCidadeQueryHandler : IRequestHandler<ObterCidadeQuery, Result<CidadeDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ObterCidadeQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CidadeDto>> Handle(
        ObterCidadeQuery request,
        CancellationToken cancellationToken)
    {
        var cidade = await _unitOfWork.Cidades.ObterPorIdAsync(request.IdCidade, cancellationToken);

        var dto = new CidadeDto(cidade.IdCidade, cidade.Nome, cidade.IdEstado);

        return Result<IEnumerable<CidadeDto>>.Success(dto);
    }
}
