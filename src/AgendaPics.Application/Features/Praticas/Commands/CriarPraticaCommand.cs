using AgendaPics.Application.Common.Mediator;
using AgendaPics.Domain.Common;
using AgendaPics.Domain.Entities;
using AgendaPics.Domain.Interfaces;

namespace AgendaPics.Application.Features.Praticas.Commands;

public record CriarPraticaCommand(string Nome, string? Descricao) : IRequest<Result<short>>;

public class CriarPraticaCommandHandler : IRequestHandler<CriarPraticaCommand, Result<short>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CriarPraticaCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<short>> Handle(CriarPraticaCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
            return Result<short>.Failure("O campo nome é obrigatório.");

        var pratica = Pratica.Criar(request.Nome, request.Descricao);
        await _unitOfWork.Praticas.AdicionarAsync(pratica, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<short>.Success(pratica.IdPratica);
    }
}
