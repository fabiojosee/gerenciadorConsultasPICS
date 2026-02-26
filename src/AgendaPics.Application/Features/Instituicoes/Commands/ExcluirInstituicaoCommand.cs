using AgendaPics.Application.Common.Mediator;
using AgendaPics.Domain.Common;
using AgendaPics.Domain.Interfaces;

namespace AgendaPics.Application.Features.Instituicoes.Commands;

public record ExcluirInstituicaoCommand(int IdInstituicao) : IRequest<Result>;

public class ExcluirInstituicaoCommandHandler : IRequestHandler<ExcluirInstituicaoCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public ExcluirInstituicaoCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ExcluirInstituicaoCommand request, CancellationToken cancellationToken)
    {
        var instituicao = await _unitOfWork.Instituicoes.ObterPorIdAsync(request.IdInstituicao, cancellationToken);

        if (instituicao == null)
            return Result.Failure("Instituição não encontrada");

        var agendamentos = await _unitOfWork.Agendamentos.ObterPorInstituicaoAsync(request.IdInstituicao, cancellationToken);
        if (agendamentos.Any())
            return Result.Failure("Não é possível excluir uma instituição com agendamentos vinculados");

        var usuario = await _unitOfWork.Usuarios.ObterPorInstituicaoAsync(request.IdInstituicao, cancellationToken);
        if (usuario != null)
        {
            _unitOfWork.Usuarios.Remover(usuario);
        }

        var praticasInstituicao = await _unitOfWork.PraticasInstituicoes.ObterPorInstituicaoAsync(request.IdInstituicao, cancellationToken);
        foreach (var praticaInstituicao in praticasInstituicao)
        {
            _unitOfWork.PraticasInstituicoes.Remover(praticaInstituicao);
        }

        _unitOfWork.Instituicoes.Remover(instituicao);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }
}
