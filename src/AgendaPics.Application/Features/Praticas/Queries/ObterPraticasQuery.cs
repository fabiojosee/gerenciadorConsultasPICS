using AgendaPics.Application.Common.Mediator;
using AgendaPics.Domain.Common;
using AgendaPics.Domain.Interfaces;

namespace AgendaPics.Application.Features.Praticas.Queries;

public record ObterPraticasQuery(int? IdInstituicao = null) : IRequest<Result<IEnumerable<PraticaDto>>>;

public record PraticaDto(
    short IdPratica,
    string Nome,
    string? Descricao);

public class ObterPraticasQueryHandler : IRequestHandler<ObterPraticasQuery, Result<IEnumerable<PraticaDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ObterPraticasQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<PraticaDto>>> Handle(
        ObterPraticasQuery request,
        CancellationToken cancellationToken)
    {
        if (request.IdInstituicao.HasValue)
        {
            var praticasInstituicao = await _unitOfWork.PraticasInstituicoes
                .ObterPorInstituicaoAsync(request.IdInstituicao.Value, cancellationToken);

            var dtos = new List<PraticaDto>();
            foreach (var pi in praticasInstituicao)
            {
                var pratica = await _unitOfWork.Praticas.ObterPorIdAsync(pi.IdPratica, cancellationToken);
                if (pratica != null)
                {
                    dtos.Add(new PraticaDto(pratica.IdPratica, pratica.Nome, pratica.Descricao));
                }
            }

            return Result<IEnumerable<PraticaDto>>.Success(dtos);
        }

        var praticas = await _unitOfWork.Praticas.ObterTodosAsync(cancellationToken);
        var allDtos = praticas.Select(p => new PraticaDto(p.IdPratica, p.Nome, p.Descricao));

        return Result<IEnumerable<PraticaDto>>.Success(allDtos);
    }
}
