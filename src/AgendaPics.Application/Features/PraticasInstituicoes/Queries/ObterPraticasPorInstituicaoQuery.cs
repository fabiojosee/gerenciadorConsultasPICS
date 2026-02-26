using AgendaPics.Application.Common.Mediator;
using AgendaPics.Domain.Common;
using AgendaPics.Domain.Interfaces;

namespace AgendaPics.Application.Features.PraticasInstituicoes.Queries;

public record ObterPraticaInstituicaoQuery(short IdPratica, int IdInstituicao) : IRequest<Result<PraticaInstituicaoDto>>;

public record PraticaInstituicaoDto(
    short IdPratica,
    int IdInstituicao,
    byte DiaPermitidoParaAgendamento,
    short QtdSessoes,
    byte Periodicidade);

public class ObterPraticasInstituicaoQueryHandler : IRequestHandler<ObterPraticaInstituicaoQuery, Result<PraticaInstituicaoDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ObterPraticasInstituicaoQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PraticaInstituicaoDto>> Handle(
        ObterPraticaInstituicaoQuery request,
        CancellationToken cancellationToken)
    {
        var praticaInstituicao = await _unitOfWork.PraticasInstituicoes.ObterPorPraticaInstituicaoAsync(request.IdInstituicao, request.IdPratica, cancellationToken);
        if (praticaInstituicao is null)
            return Result<PraticaInstituicaoDto>.Failure("Prática da instituição não encontrada.");

        var dto = new PraticaInstituicaoDto(request.IdPratica, request.IdInstituicao, praticaInstituicao.DiaPermitidoParaAgendamento, praticaInstituicao.QtdSessoes, praticaInstituicao.Periodicidade);
        return Result<PraticaInstituicaoDto>.Success(dto);
    }
}
