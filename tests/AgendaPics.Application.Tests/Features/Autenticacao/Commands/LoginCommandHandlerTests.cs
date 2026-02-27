using AgendaPics.Application.Features.Autenticacao.Commands;
using AgendaPics.Application.Tests.Common;
using FluentAssertions;
using Moq;

namespace AgendaPics.Application.Tests.Features.Autenticacao.Commands;

public class LoginCommandHandlerTests : HandlerTestBase
{
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _handler = new LoginCommandHandler(UnitOfWorkMock.Object, PasswordHasherMock.Object);
    }

    [Fact]
    public async Task Handle_LoginVazio_RetornaFalha()
    {
        var command = new LoginCommand("", "senha123");

        var result = await _handler.Handle(command, Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Login");
    }

    [Fact]
    public async Task Handle_SenhaVazia_RetornaFalha()
    {
        var command = new LoginCommand("admin", "");

        var result = await _handler.Handle(command, Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Senha");
    }

    [Fact]
    public async Task Handle_UsuarioNaoEncontrado_RetornaFalha()
    {
        UsuariosMock.Setup(r => r.ObterPorLoginAsync("admin", Ct)).ReturnsAsync((Domain.Entities.Usuario?)null);

        var command = new LoginCommand("admin", "senha123");

        var result = await _handler.Handle(command, Ct);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("inválidos");
    }

    [Fact]
    public async Task Handle_SenhaBcryptCorreta_RetornaLoginResult()
    {
        var usuario = EntityBuilders.CriarUsuario(login: "admin", senha: "$2a$hash");
        UsuariosMock.Setup(r => r.ObterPorLoginAsync("admin", Ct)).ReturnsAsync(usuario);
        PasswordHasherMock.Setup(p => p.IsLegacyHash("$2a$hash")).Returns(false);
        PasswordHasherMock.Setup(p => p.Verify("senha123", "$2a$hash")).Returns(true);

        var command = new LoginCommand("admin", "senha123");

        var result = await _handler.Handle(command, Ct);

        result.IsSuccess.Should().BeTrue();
        result.Value.Login.Should().Be("admin");
        result.Value.IdPerfil.Should().Be(usuario.IdPerfil);
    }

    [Fact]
    public async Task Handle_SenhaBcryptIncorreta_RetornaFalha()
    {
        var usuario = EntityBuilders.CriarUsuario(login: "admin", senha: "$2a$hash");
        UsuariosMock.Setup(r => r.ObterPorLoginAsync("admin", Ct)).ReturnsAsync(usuario);
        PasswordHasherMock.Setup(p => p.IsLegacyHash("$2a$hash")).Returns(false);
        PasswordHasherMock.Setup(p => p.Verify("errada", "$2a$hash")).Returns(false);

        var command = new LoginCommand("admin", "errada");

        var result = await _handler.Handle(command, Ct);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_SenhaLegadaCorreta_MigraParaBcryptERetornaSucesso()
    {
        // SHA-256 de "senha123"
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes("senha123"));
        var legacyHash = string.Concat(bytes.Select(b => b.ToString("x2")));

        var usuario = EntityBuilders.CriarUsuario(login: "admin", senha: legacyHash);
        UsuariosMock.Setup(r => r.ObterPorLoginAsync("admin", Ct)).ReturnsAsync(usuario);
        PasswordHasherMock.Setup(p => p.IsLegacyHash(legacyHash)).Returns(true);
        PasswordHasherMock.Setup(p => p.Hash("senha123")).Returns("$2a$novohash");

        var command = new LoginCommand("admin", "senha123");

        var result = await _handler.Handle(command, Ct);

        result.IsSuccess.Should().BeTrue();
        PasswordHasherMock.Verify(p => p.Hash("senha123"), Times.Once);
        UsuariosMock.Verify(r => r.Atualizar(usuario), Times.Once);
        UnitOfWorkMock.Verify(u => u.CommitAsync(Ct), Times.Once);
    }

    [Fact]
    public async Task Handle_SenhaLegadaIncorreta_NaoMigraERetornaFalha()
    {
        var usuario = EntityBuilders.CriarUsuario(login: "admin", senha: "hashsha256antigo");
        UsuariosMock.Setup(r => r.ObterPorLoginAsync("admin", Ct)).ReturnsAsync(usuario);
        PasswordHasherMock.Setup(p => p.IsLegacyHash("hashsha256antigo")).Returns(true);

        var command = new LoginCommand("admin", "senhaerrada");

        var result = await _handler.Handle(command, Ct);

        result.IsFailure.Should().BeTrue();
        UnitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_PrimeiroAcesso_RetornaFlPrimeiroAcessoTrue()
    {
        var usuario = EntityBuilders.CriarUsuario(login: "admin", senha: "$2a$hash", flPrimeiroAcesso: true);
        UsuariosMock.Setup(r => r.ObterPorLoginAsync("admin", Ct)).ReturnsAsync(usuario);
        PasswordHasherMock.Setup(p => p.IsLegacyHash("$2a$hash")).Returns(false);
        PasswordHasherMock.Setup(p => p.Verify("senha123", "$2a$hash")).Returns(true);

        var command = new LoginCommand("admin", "senha123");

        var result = await _handler.Handle(command, Ct);

        result.IsSuccess.Should().BeTrue();
        result.Value.FlPrimeiroAcesso.Should().BeTrue();
    }
}
