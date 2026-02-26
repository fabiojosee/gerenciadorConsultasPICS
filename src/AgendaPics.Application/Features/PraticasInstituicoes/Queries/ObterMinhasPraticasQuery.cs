using AgendaPics.Application.Common.Mediator;
using AgendaPics.Domain.Common;
using AgendaPics.Domain.Interfaces;

namespace AgendaPics.Application.Features.PraticasInstituicoes.Queries;

public record ObterMinhasPraticasQuery(int IdInstituicao) : IRequest<Result<IEnumerable<MinhasPraticasDto>>>;

public record MinhasPraticasDto(
    short IdPratica,
    string Nome,
    string? Descricao,
    byte Periodicidade,
    short QtdSessoes,
    byte? DiaPermitidoParaAgendamento);

public class ObterMinhasPraticasQueryHandler : IRequestHandler<ObterMinhasPraticasQuery, Result<IEnumerable<MinhasPraticasDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ObterMinhasPraticasQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<MinhasPraticasDto>>> Handle(
        ObterMinhasPraticasQuery request,
        CancellationToken cancellationToken)
    {
        var praticasInstituicao = await _unitOfWork.PraticasInstituicoes
            .ObterPorInstituicaoAsync(request.IdInstituicao, cancellationToken);

        var result = new List<MinhasPraticasDto>();
        foreach (var pi in praticasInstituicao)
        {
            var pratica = await _unitOfWork.Praticas.ObterPorIdAsync(pi.IdPratica, cancellationToken);
            if (pratica != null)
            {
                result.Add(new MinhasPraticasDto(
                    pratica.IdPratica,
                    pratica.Nome,
                    pratica.Descricao,
                    pi.Periodicidade,
                    pi.QtdSessoes,
                    pi.DiaPermitidoParaAgendamento));
            }
        }

        return Result<IEnumerable<MinhasPraticasDto>>.Success(result);
    }
}
