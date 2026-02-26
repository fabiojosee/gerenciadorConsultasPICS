using AgendaPics.Application.Common.Mediator;
using AgendaPics.Domain.Common;
using AgendaPics.Domain.Interfaces;

namespace AgendaPics.Application.Features.Praticas.Commands;

public record EditarPraticaCommand(short IdPratica, string Nome, string? Descricao) : IRequest<Result<bool>>;

public class EditarPraticaCommandHandler : IRequestHandler<EditarPraticaCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public EditarPraticaCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(EditarPraticaCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
            return Result<bool>.Failure("O campo nome é obrigatório.");

        var pratica = await _unitOfWork.Praticas.ObterPorIdAsync(request.IdPratica, cancellationToken);
        if (pratica is null)
            return Result<bool>.Failure("Prática não encontrada.");

        pratica.Atualizar(request.Nome, request.Descricao);
        await _unitOfWork.Praticas.AtualizarAsync(pratica, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
