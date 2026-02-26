using AgendaPics.Application.Common.Interfaces;
using AgendaPics.Application.Common.Mediator;
using AgendaPics.Domain.Common;
using AgendaPics.Domain.Entities;
using AgendaPics.Domain.Interfaces;

namespace AgendaPics.Application.Features.Autenticacao.Commands;

public record LoginCommand(string Login, string Senha) : IRequest<Result<LoginResult>>;

public record LoginResult(
    int IdUsuario,
    string Login,
    byte IdPerfil,
    int? IdInstituicao,
    bool FlPrimeiroAcesso);

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResult>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public LoginCommandHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<LoginResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Login))
            return Result<LoginResult>.Failure("Login é obrigatório");

        if (string.IsNullOrWhiteSpace(request.Senha))
            return Result<LoginResult>.Failure("Senha é obrigatória");

        var usuario = await _unitOfWork.Usuarios.ObterPorLoginAsync(request.Login, cancellationToken);

        if (usuario == null)
            return Result<LoginResult>.Failure("Usuário ou senha inválidos");

        var senhaCorreta = false;
        // Verifica se está usando hash legado (SHA256)
        if (_passwordHasher.IsLegacyHash(usuario.Senha))
        {
            var legacyHash = ComputeLegacySha256Hash(request.Senha);
            senhaCorreta = usuario.Senha == legacyHash;

            if (senhaCorreta)
            {
                // Migrar para BCrypt
                var newHash = _passwordHasher.Hash(request.Senha);
                usuario.AlterarSenha(newHash);
                _unitOfWork.Usuarios.Atualizar(usuario);
                await _unitOfWork.CommitAsync(cancellationToken);
            }
        }
        else
        {
            senhaCorreta = _passwordHasher.Verify(request.Senha, usuario.Senha);
        }

        if (!senhaCorreta)
            return Result<LoginResult>.Failure("Usuário ou senha inválidos");

        return Result<LoginResult>.Success(new LoginResult(
            usuario.IdUsuario,
            usuario.Login,
            usuario.IdPerfil,
            usuario.IdInstituicao,
            usuario.FlPrimeiroAcesso
        ));
    }

    private static string ComputeLegacySha256Hash(string input)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
        var sb = new System.Text.StringBuilder();
        foreach (var b in bytes)
        {
            sb.Append(b.ToString("x2"));
        }
        return sb.ToString();
    }
}
