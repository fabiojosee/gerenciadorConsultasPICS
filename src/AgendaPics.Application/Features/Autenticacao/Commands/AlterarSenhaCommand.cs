using AgendaPics.Application.Common.Interfaces;
using AgendaPics.Application.Common.Mediator;
using AgendaPics.Domain.Common;
using AgendaPics.Domain.Entities;
using AgendaPics.Domain.Interfaces;

namespace AgendaPics.Application.Features.Autenticacao.Commands;

public record AlterarSenhaCommand(
    int? IdInstituicao,
    string? Login,
    string NovaSenha,
    string ConfirmacaoSenha,
    bool IsPrimeiroAcesso = false) : IRequest<Result>;

public class AlterarSenhaCommandHandler : IRequestHandler<AlterarSenhaCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public AlterarSenhaCommandHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result> Handle(AlterarSenhaCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.NovaSenha))
            return Result.Failure("Nova senha é obrigatória");

        if (request.NovaSenha.Length < 6)
            return Result.Failure("A senha deve ter no mínimo 6 caracteres");

        if (request.NovaSenha != request.ConfirmacaoSenha)
            return Result.Failure("As senhas não conferem");

        Usuario? usuario = request.IdInstituicao.HasValue
            ? await _unitOfWork.Usuarios.ObterPorInstituicaoAsync(request.IdInstituicao.Value, cancellationToken)
            : await _unitOfWork.Usuarios.ObterPorLoginAsync(request.Login!, cancellationToken);
        if (usuario == null)
            return Result.Failure("Usuário não encontrado");

        var senhaHash = _passwordHasher.Hash(request.NovaSenha);
        usuario.AlterarSenha(senhaHash);

        if (request.IsPrimeiroAcesso)
            usuario.AlterarFlagPrimeiroAcesso(false);

        _unitOfWork.Usuarios.Atualizar(usuario);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }
}
