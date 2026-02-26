using AgendaPics.Application.Common.Mediator;
using AgendaPics.Domain.Common;
using AgendaPics.Domain.Interfaces;

namespace AgendaPics.Application.Features.Praticas.Commands;

public record ExcluirPraticaCommand(short IdPratica) : IRequest<Result<bool>>;

public class ExcluirPraticaCommandHandler : IRequestHandler<ExcluirPraticaCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ExcluirPraticaCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(ExcluirPraticaCommand request, CancellationToken cancellationToken)
    {
        var agendamentos = await _unitOfWork.Agendamentos.ObterPorPraticaAsync(request.IdPratica, cancellationToken);
        if (agendamentos.Any())
            return Result<bool>.Failure("A prática não pode ser excluída, pois já existem agendamentos associados a ela.");

        var vinculos = await _unitOfWork.PraticasInstituicoes.ObterInstituicoesVinculadasAsync(request.IdPratica, cancellationToken);
        foreach (var vinculo in vinculos)
        {
            await _unitOfWork.PraticasInstituicoes.RemoverAsync(vinculo.IdPratica, vinculo.IdInstituicao, cancellationToken);
        }

        await _unitOfWork.Praticas.RemoverAsync(request.IdPratica, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
