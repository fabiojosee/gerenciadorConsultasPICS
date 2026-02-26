using AgendaPics.Application.Common.Mediator;
using AgendaPics.Domain.Common;
using AgendaPics.Domain.Interfaces;

namespace AgendaPics.Application.Features.Instituicoes.Queries;

public record ObterInstituicoesQuery(int? IdCidade = null, short? IdEstado = null) : IRequest<Result<IEnumerable<InstituicaoDto>>>;

public record InstituicaoDto(
    int IdInstituicao,
    string Nome,
    string? Descricao,
    string Email,
    string Cnpj,
    string Cep,
    int IdCidade,
    TimeSpan HorarioInicioAtendimento,
    TimeSpan HorarioFimAtendimento);

public class ObterInstituicoesQueryHandler : IRequestHandler<ObterInstituicoesQuery, Result<IEnumerable<InstituicaoDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ObterInstituicoesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<InstituicaoDto>>> Handle(
        ObterInstituicoesQuery request,
        CancellationToken cancellationToken)
    {
        IEnumerable<Domain.Entities.Instituicao> instituicoes;

        if (request.IdCidade.HasValue)
        {
            instituicoes = await _unitOfWork.Instituicoes.ObterPorCidadeAsync(request.IdCidade.Value, cancellationToken);
        }
        else if (request.IdEstado.HasValue)
        {
            instituicoes = await _unitOfWork.Instituicoes.ObterPorEstadoAsync(request.IdEstado.Value, cancellationToken);
        }
        else
        {
            instituicoes = await _unitOfWork.Instituicoes.ObterTodosAsync(cancellationToken);
        }

        var dtos = instituicoes.Select(i => new InstituicaoDto(
            i.IdInstituicao,
            i.Nome,
            i.Descricao,
            i.Email,
            i.Cnpj,
            i.Cep,
            i.IdCidade,
            i.HorarioInicioAtendimento,
            i.HorarioFimAtendimento
        ));

        return Result<IEnumerable<InstituicaoDto>>.Success(dtos);
    }
}
