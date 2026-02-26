using AgendaPics.Application.Common.Mediator;
using AgendaPics.Domain.Common;
using AgendaPics.Domain.Interfaces;
using AgendaPics.Domain.ValueObjects;

namespace AgendaPics.Application.Features.Instituicoes.Commands;

public record EditarInstituicaoCommand(
    int IdInstituicao,
    string Nome,
    string? Descricao,
    short IdEstado,
    int IdCidade,
    string Cnpj,
    string Cep,
    string Email,
    TimeSpan HorarioInicioAtendimento,
    TimeSpan HorarioFimAtendimento) : IRequest<Result>;

public class EditarInstituicaoCommandHandler : IRequestHandler<EditarInstituicaoCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public EditarInstituicaoCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(EditarInstituicaoCommand request, CancellationToken cancellationToken)
    {
        var cnpjResult = CNPJ.Create(request.Cnpj);
        if (cnpjResult.IsFailure)
            return Result.Failure(cnpjResult.Error);

        var emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
            return Result.Failure(emailResult.Error);

        var instituicao = await _unitOfWork.Instituicoes.ObterPorIdAsync(request.IdInstituicao, cancellationToken);

        if (instituicao == null)
            return Result.Failure("Instituição não encontrada");

        if (!instituicao.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase))
        {
            var instituicaoComEmail = await _unitOfWork.Instituicoes.ObterPorEmailAsync(request.Email, cancellationToken);
            if (instituicaoComEmail != null)
                return Result.Failure("E-mail já cadastrado");
        }

        instituicao.Atualizar(
            request.Nome,
            request.Descricao,
            request.IdCidade,
            cnpjResult.Value.Value,
            request.Cep,
            emailResult.Value.Value,
            request.HorarioInicioAtendimento,
            request.HorarioFimAtendimento);

        _unitOfWork.Instituicoes.Atualizar(instituicao);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }
}
